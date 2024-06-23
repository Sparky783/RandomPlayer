using NReco.VideoInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NReco.VideoInfo.MediaInfo;

namespace RandomPlayer.Models
{
    /// <summary>
    /// Format file metadata into an object
    /// </summary>
    public class Metadata
    {
        #region Properties
        public MediaInfo MediaInfo { get; set; }

        public string Duration { get; set; }
        public string VideoCodec { get; set; }
        public string AudioCodec { get; set; }
        public string Size { get; set; }
        #endregion

        public Metadata(MediaInfo info)
        {
            MediaInfo = info;

            Duration = MediaInfo.Duration.ToString(@"hh\:mm\:s");

            foreach (StreamInfo stream in MediaInfo.Streams)
            {
                switch (stream.CodecType)
                {
                    case "video":
                        VideoCodec = stream.CodecName;
                        Size = stream.Width + "x" + stream.Height;
                        break;

                    case "audio":
                        AudioCodec = stream.CodecName;
                        break;
                }
            }
        }

        private void LogInConsole()
        {
            string message = "";
            message += string.Format("File format: {0}", MediaInfo.FormatName) + "\n";
            message += string.Format("Duration: {0}", MediaInfo.Duration) + "\n\n";

            foreach (KeyValuePair<string, string> tag in MediaInfo.FormatTags)
                message += string.Format("\t{0}: {1}", tag.Key, tag.Value) + "\n";

            foreach (StreamInfo stream in MediaInfo.Streams)
            {
                message += string.Format("Stream {0} ({1})", stream.CodecName, stream.CodecType) + "\n";

                if (stream.CodecType == "video")
                {
                    message += string.Format("\tFrame size: {0}x{1}", stream.Width, stream.Height) + "\n";
                    message += string.Format("\tFrame rate: {0:0.##}", stream.FrameRate) + "\n";
                }

                foreach (KeyValuePair<string, string> tag in stream.Tags)
                    message += string.Format("\t{0}: {1}", tag.Key, tag.Value) + "\n";

                message += "\n";
            }

            Console.WriteLine(message);
        }
    }
}
