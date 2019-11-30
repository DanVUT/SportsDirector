using MFORMATSLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace MFormat.Model
{


    public class BroadcastManager : INotifyPropertyChanged
    {
        private object mediaLock = new Object();
        private object deviceLock = new object();
        private object streamLock = new object();
        private object broadcastRenderLock = new object();
        private object liveRenderLock = new object();
        public class StreamRecorder
        {
            public StreamRecorder(M_AV_PROPS avProps, MFWriterClass writer, string destinationFile, string recordingOptions)
            {
                this.RecorderAVprops = avProps;
                this.Writer = writer;
                this.DestinationFile = destinationFile;
                this.RecordingOptions = recordingOptions;
            }
            public M_AV_PROPS RecorderAVprops { get; set; }
            public MFWriterClass Writer { get; set; }
            public string DestinationFile { get; set; }
            public string RecordingOptions { get; set; }
        }
        //Frame buffer used for recording
        private Queue<MFFrame> frameBuffer = new Queue<MFFrame>();

        //Enumeration of possible input sources
        enum Sources
        {
            NoSource,
            DeviceSource,
            StreamSource,
        }
        private Sources currentSource; 
        //Preview Rendering class
        private MFPreviewClass broadcastPreview;
        //Preview Rendering class
        private MFPreviewClass livePreview;
        //Media stream reader (File streams)
        private MFReaderClass mediaReader;
        private bool IsImage { get; set; } = false;
        //URL stream reader
        private MFReaderClass streamReader;
        //Live device reader
        private MFLiveClass deviceReader;
        //Stream recorder
        public StreamRecorder recorder { get; private set; } = null;
        //Flag telling whether playback thread is running or not
        private bool IsLiveSourcePlaying { get; set; } = false;
        private bool IsMediaPlaying { get; set; } = false;
        //Flag telling whether there is stream/live device input
        public bool IsStreaming { get { return streamReader != null || deviceReader != null; } }
        //Flag telling whether stream input is recording
        public bool IsRecording { get; private set; } = false;
        private int _garbageCounter;
        //Live input informations
        private M_AV_PROPS mediaProps;
        //If media is supposed to play from certain time until certain time this represents end time. If media should be play whole it has value -1
        private double playbackEndTime;
        private int mediaHeight;
        private int mediaWidth;

        //Preview renderer renders frame on this Image Source and this ImageSource is used for Image Preview in the destination place
        D3DImage broadcastPreviewSource;
        public D3DImage BroadcastPreviewSource
        {
            get { return broadcastPreviewSource; }
            set { broadcastPreviewSource = value; }
        }

        D3DImage livePreviewSource;

        public D3DImage LivePreviewSource
        {
            get { return livePreviewSource; }
            set { livePreviewSource = value; }
        }
        //Constructor initializes Preview Class for two WPF preview type and creates a new render target
        public BroadcastManager(out D3DImage livePreviewSource, out D3DImage broadcastPreviewSource,int enableSound = 1)
        {
            InitializeLivePreview(0);
            InitializeBroadcastPreview(enableSound);
            livePreviewSource = new D3DImage();
            this.livePreviewSource = livePreviewSource;
            broadcastPreviewSource = new D3DImage();
            this.broadcastPreviewSource = broadcastPreviewSource;
            currentSource = Sources.NoSource;
        }

        //Constructor initializes Preview Class for one WPF preview type and creates a new render target
        public BroadcastManager(out D3DImage broadcastPreviewSource, int enableSound = 1)
        {
            InitializeBroadcastPreview(enableSound);
            broadcastPreviewSource = new D3DImage();
            this.BroadcastPreviewSource = broadcastPreviewSource;
            currentSource = Sources.NoSource;
        }


        #region InitializePlayer
        private void InitializeLivePreview(int enableSound)
        {
            livePreview = new MFPreviewClass();
            livePreview.PropsSet("wpf_preview", "true");
            livePreview.PropsSet("wpf_preview.update", "0");
            livePreview.PropsSet("wpf_preview.sync_texture", "true");
            livePreview.PropsSet("on_frame.sync", "true");
            livePreview.PropsSet("on_event.sync", "true");
            livePreview.PreviewEnable("", 0, 1);
            livePreview.OnEventSafe += livePreview_OnEventSafe;
        }
        //Generic Initialization of MFPreview for WPF style of preview
        private void InitializeBroadcastPreview(int enableSound)
        {
            broadcastPreview = new MFPreviewClass();
            broadcastPreview.PropsSet("wpf_preview", "true");
            broadcastPreview.PropsSet("wpf_preview.update", "0");
            broadcastPreview.PropsSet("wpf_preview.sync_texture", "true");
            broadcastPreview.PropsSet("on_frame.sync", "true");
            broadcastPreview.PropsSet("on_event.sync", "true");
            broadcastPreview.PreviewEnable("", enableSound, 1);
            broadcastPreview.OnEventSafe += broadcastPreview_OnEventSafe;
        }
        //Getter for rendering target that is used in final destination

        //Generic Event handler for Preview Class. When new frame is put into renderer it will be rendered into BroadcastPreviewSource property
        private IntPtr _mSavedPointer;
        private void broadcastPreview_OnEventSafe(string bsChannelId, string bsEventName, string bsEventParam, object pEventObject)
        {
            lock (broadcastRenderLock)
            {
                if (bsEventName == "wpf_nextframe")
                {
                    IntPtr pEventObjectPtr = Marshal.GetIUnknownForObject(pEventObject);
                    if (pEventObjectPtr != _mSavedPointer)
                    {
                        if (_mSavedPointer != IntPtr.Zero)
                            Marshal.Release(_mSavedPointer);

                        _mSavedPointer = pEventObjectPtr;

                        BroadcastPreviewSource.Lock();
                        BroadcastPreviewSource.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                        BroadcastPreviewSource.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _mSavedPointer);
                        BroadcastPreviewSource.Unlock();
                        OnPropertyChanged("BroadcastPreviewSource");
                    }
                    if (pEventObjectPtr != IntPtr.Zero)
                        Marshal.Release(pEventObjectPtr);

                    BroadcastPreviewSource.Lock();
                    BroadcastPreviewSource.AddDirtyRect(new Int32Rect(0, 0, broadcastPreviewSource.PixelWidth, broadcastPreviewSource.PixelHeight));
                    BroadcastPreviewSource.Unlock();
                    
                }
                Marshal.ReleaseComObject(pEventObject);
            }
        }
        #endregion

        private IntPtr mSavedPointer;
        private void livePreview_OnEventSafe(string bsChannelId, string bsEventName, string bsEventParam, object pEventObject)
        {
            lock (liveRenderLock) {
                if (bsEventName == "wpf_nextframe")
                {
                    IntPtr pEventObjectPtr = Marshal.GetIUnknownForObject(pEventObject);
                    if (pEventObjectPtr != mSavedPointer)
                    {
                        if (mSavedPointer != IntPtr.Zero)
                            Marshal.Release(mSavedPointer);

                        mSavedPointer = pEventObjectPtr;

                        LivePreviewSource.Lock();
                        LivePreviewSource.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                        LivePreviewSource.SetBackBuffer(D3DResourceType.IDirect3DSurface9, mSavedPointer);
                        LivePreviewSource.Unlock();
                        OnPropertyChanged("LivePreviewSource");
                    }
                    if (pEventObjectPtr != IntPtr.Zero)
                        Marshal.Release(pEventObjectPtr);

                    LivePreviewSource.Lock();
                    LivePreviewSource.AddDirtyRect(new Int32Rect(0, 0, livePreviewSource.PixelWidth, livePreviewSource.PixelHeight));
                    LivePreviewSource.Unlock();
                }
                Marshal.ReleaseComObject(pEventObject);
            }
        }

        //Method starts playing live video in the next thread
        public void StartLiveSourcePlayback()
        {
            if (currentSource == Sources.NoSource)
            {
                MessageBox.Show("Select stream or video device source first");
                return;
            }
            if (!IsLiveSourcePlaying)
            {
                IsLiveSourcePlaying = true;
                Task PlaybackThread = new Task(LiveSourceThread);
                PlaybackThread.Start();
            }
        }
        //Live Source Thread reads from selected input source and puts frames into preview renderer
        private void LiveSourceThread()
        {
            while (IsLiveSourcePlaying)
            {
                try
                {
                    if (currentSource == Sources.NoSource)
                    {
                        IsLiveSourcePlaying = false;
                        break;
                    }
                    int rest;
                    MFFrame sourceFrame = null;
                    MFFrame overlayFrame = null;
                    MFFactory overlayFactory = new MFFactory();

                    switch (this.currentSource)
                    {
                        //If stream source is playing
                        case Sources.StreamSource:
                            //gets frame from stream
                            sourceFrame = GetFrameFromStream();
                            break;
                        //If live device is playing
                        case Sources.DeviceSource:
                            //Reads frame from Live device
                            sourceFrame = GetFrameFromLiveDevice();
                            break;
                    }
                    livePreview.ReceiverFramePut(sourceFrame, -1, "");
                    if (!IsMediaPlaying)
                    {
                        broadcastPreview.ReceiverFramePut(sourceFrame, -1, "");
                    }
                    if (this.IsRecording)
                    {
                        //If recording is active, put this frame to frameBuffer, so recorder can write it
                        this.frameBuffer.Enqueue(sourceFrame);
                    }
                    if (!IsRecording)
                    {
                        Marshal.ReleaseComObject(sourceFrame);
                    }
                    //Marshal.ReleaseComObject(overlayFrame);
                    _garbageCounter++;
                    if (_garbageCounter % 10 == 0)
                        GC.Collect();
                }
                catch (Exception)
                {
                    Thread.Sleep(1);
                }
            }
        }
        public void StartMediaSourcePlayback()
        {
            if(this.mediaReader == null)
            {
                MessageBox.Show("Select file, stream or video device source first");
                return;
            }

            if (!IsMediaPlaying)
            {
                IsMediaPlaying = true;
                Task PlaybackThread = new Task(MediaSourceThread);
                PlaybackThread.Start();
            }
        }
        //Method sets media on certain time, and sets endtime when playback should end
        public void StartMediaSourcePlayback(double from, double until)
        {
            SetMediaToTime(from);
            this.playbackEndTime = until;
            StartMediaSourcePlayback();
        }
        //Help method setting start of current media on given time
        public void SetMediaToTime(double time)
        {
            MFFrame sourceFrame;
            this.mediaReader.SourceFrameGetByTime(time, -1, out sourceFrame, "");
        }
        public void StartMediaSourcePlayback(double length)
        {
            length = length * 1000;
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.AutoReset = false;
            timer.Interval = length;
            timer.Elapsed += (s, e) =>
            {
                StopMediaSourcePlayback();
                OnMediaFinished();
                timer.Stop();
            };
            StartMediaSourcePlayback();
            timer.Start();
        }

        public void MediaSourceThread()
        {
            while (IsMediaPlaying)
            {
                try
                {
                    MFFrame sourceFrame = GetFrameFromMedia();
                    this.broadcastPreview.ReceiverFramePut(sourceFrame, -1, "");
                    M_TIME mediaTime;
                    sourceFrame.MFTimeGet(out mediaTime);
                    //Condition checking if maximum playtime was set and if yes, ends the video
                    if (this.playbackEndTime > 0)
                    {
                        if (mediaTime.rtEndTime > this.playbackEndTime * 10000000)
                        {
                            this.IsMediaPlaying = false;
                            this.playbackEndTime = -1;
                        }
                    }
                    //Condition checking end of media file
                    if ((mediaTime.eFFlags & eMFrameFlags.eMFF_Last) != 0 && !this.IsImage)
                    {
                        this.IsMediaPlaying = false;
                        this.mediaReader.ReaderClose();
                        this.mediaReader = null;
                        OnMediaFinished();
                    }
                    Marshal.ReleaseComObject(sourceFrame);
                }
                catch
                {
                    Thread.Sleep(1);
                }
            }
        }

        //Stops playing all videos
        public void StopPlayback()
        {
            IsMediaPlaying = false;
            IsLiveSourcePlaying = false;
        }
        //Stops playing media source video
        public void StopMediaSourcePlayback()
        {
            IsMediaPlaying = false;
            if (this.mediaReader != null)
            {
                lock (this.mediaLock)
                {
                    this.mediaReader.ReaderClose();
                    this.mediaReader = null;
                }
            }
        }
        //Method reads next frame from local media
        private MFFrame GetFrameFromMedia()
        {
            MFFrame sourceFrame;
            lock (this.mediaLock)
            {
                mediaReader.SourceFrameGetByTime(-1, -1, out sourceFrame, "");
            }
            return sourceFrame;
        }
        //Gets frame from URL stream
        private MFFrame GetFrameFromStream()
        {
            MFFrame sourceFrame;
            lock (this.streamLock)
            {
                streamReader.SourceFrameGet(-1, out sourceFrame, "");
            }
            return sourceFrame;
        }
        //Gets frame from live device (Directly connected to PC)
        private MFFrame GetFrameFromLiveDevice()
        {
            MFFrame sourceFrame;
            lock (this.deviceLock)
            {
                deviceReader.SourceFrameGet(-1, out sourceFrame, "");
            }
            return sourceFrame;
        }
        
        //This method changes main live video source to device
        public void ChangeDevice(MFLiveClass deviceReader)
        {
            try
            {
                lock (this.deviceLock)
                {
                    if (this.deviceReader != null)
                    {
                        this.deviceReader.DeviceClose();
                    }
                    if (this.streamReader != null)
                    {
                        this.streamReader.ReaderClose();
                    }
                    this.streamReader = null;
                    this.deviceReader = deviceReader;
                }
                this.currentSource = Sources.DeviceSource;
                M_AV_PROPS avProps;
                deviceReader.SourceAVPropsGet(out avProps);
                this.mediaHeight = avProps.vidProps.nHeight;
                this.mediaWidth = avProps.vidProps.nWidth;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem with setting the device:" + Environment.NewLine + ex.Message);
                mediaReader = null;
            }
        }
        //This method changes main live video source to URL stream
        public void ChangeStream(string Url)
        {
            try
            {
                lock(this.streamLock){
                    if (this.streamReader != null)
                    {
                        this.streamReader.ReaderClose();
                    }
                    this.streamReader = new MFReaderClass();
                    this.streamReader.ReaderOpen(Url, "loop=false");
                    this.currentSource = Sources.StreamSource;


                    if (this.deviceReader != null)
                    {
                        this.deviceReader.DeviceClose();
                    }
                    this.deviceReader = null;
                }
                M_AV_PROPS avProps;
                this.streamReader.SourceAVPropsGet(out avProps);
                this.mediaWidth = avProps.vidProps.nWidth;
                this.mediaHeight = avProps.vidProps.nHeight;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't open stream:" + Environment.NewLine + ex.Message);
                mediaReader = null;
            }
        }
        //This method changes main source to File source
        public void ChangeMedia(string Uri, bool IsImage = false)
        {
            try
            {
                lock (this.mediaLock)
                {
                    if (this.mediaReader != null)
                    {
                        this.mediaReader.ReaderClose();
                    }
                    this.playbackEndTime = -1;
                    this.mediaReader = new MFReaderClass();
                    if (IsImage)
                    {
                        this.mediaReader.ReaderOpen(Uri, "loop=true");
                    } else
                    {
                        this.mediaReader.ReaderOpen(Uri, "loop=false");
                    }
                    this.IsImage = IsImage;
                this.mediaReader.SourceAVPropsGet(out this.mediaProps);
                this.mediaWidth = this.mediaProps.vidProps.nWidth;
                this.mediaHeight = this.mediaProps.vidProps.nHeight;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't open the file:" + Environment.NewLine + ex.Message);
                mediaReader = null;
            }
        }
        
        //Setting recorder instance
        public void SetRecorder(MFWriterClass writer, M_AV_PROPS avProps, string destinationFile, string options)
        {
            this.recorder = new StreamRecorder(avProps, writer, destinationFile, options);
        }
        //Starts recording thread
        public void StartRecording()
        {
            this.recorder.Writer.WriterSet(this.recorder.DestinationFile, 0, this.recorder.RecordingOptions);
            this.IsRecording = true;
            Task recordingThread = new Task(RecordingThread);
            recordingThread.Start();
        }
        //Stops recording thread
        public void StopRecording()
        {
            this.IsRecording = false;
        }
        //Recording thread. Writes what is in the frame buffer. If current video source is local media file it reads the live source directly
        private void RecordingThread()
        {
            while(IsRecording || frameBuffer.Count > 0)
            {
                if(this.frameBuffer.Count > 0)
                {
                    MFFrame frame = this.frameBuffer.Dequeue();
                    MFFrame convertedFrame;
                    int rest;
                    frame.MFConvert(this.recorder.RecorderAVprops, out convertedFrame, out rest, "", "");
                    this.recorder.Writer.ReceiverFramePut(convertedFrame, -1, "");
                    Marshal.ReleaseComObject(frame);
                    Marshal.ReleaseComObject(convertedFrame);
                }
            }
            this.recorder.Writer.WriterClose(1);
        }

        //Destroy video player instance
        public void Destroy()
        {
            this.StopRecording();
            this.StopPlayback();
            if(this.streamReader != null)
            {
                this.streamReader.ReaderClose();
            }
            if(this.mediaReader != null)
            {
                this.mediaReader.ReaderClose();
            }
            if(this.deviceReader != null)
            {
                this.deviceReader.DeviceClose();
            }
            if (this.recorder != null)
            {
                this.recorder.Writer.WriterClose(1);
            }
        }

        //Returns path to recording file
        public string GetCurrentlyRecordedMedia()
        {
            return this.recorder.DestinationFile;
        }

        #region NotifyPropertyChangedIMPLEMENTATION
        public event PropertyChangedEventHandler PropertyChanged; // default implementation of INotifyPropertyChanged interface

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public event EventHandler MediaFinished;

        protected virtual void OnMediaFinished()
        {
            var handler = MediaFinished;
            if (handler != null) handler(this, null);
        }
    }
}
