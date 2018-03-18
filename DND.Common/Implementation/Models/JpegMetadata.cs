using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DND.Common.Implementation.Models
{
    public class JpegMetadata
    {
        private string path { get; set; }
        private BitmapFrame frame;
        private BitmapMetadata Metadata;

        public JpegMetadata(string path, string physicalFolderPath = "")
        {
            this.path = path;
            if (!string.IsNullOrEmpty(physicalFolderPath))
            {
                this.Id = path.Replace(physicalFolderPath, "");
            }
            else
            {
                this.Id = path;
            }
            LoadMetadata();
        }

        private void LoadMetadata()
        {
            frame = getBitmapFrame(path);
            Metadata = (BitmapMetadata)frame.Metadata.Clone();
        }

        public string Id
        {
            get; private set;
        }

        public FileInfo FileInfo
        {
            get
            {
                var fileInfo = new FileInfo(path);
                return fileInfo;
            }
        }

        public DateTime DateTaken
        {
            get
            {
                if (!string.IsNullOrEmpty(Metadata.DateTaken))
                {
                    return DateTime.Parse(Metadata.DateTaken);
                }
                else
                {
                    return FileInfo.LastWriteTimeUtc;
                }
            }
            set { Metadata.DateTaken = value.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        public string Title
        {
            get { return Metadata.Title; }
            set { Metadata.Title = value; }
        }

        public string Subject
        {
            get { return Metadata.Subject; }
            set { Metadata.Subject = value; }
        }

        public string Comments
        {
            get { return Metadata.Comment; }
            set { Metadata.Comment = value; }
        }

        public string Location
        {
            get { return Metadata.Location; }
        }

        public double? Latitude
        {
            get
            {
                try
                {
                    var latitudeRefQuery = Metadata.GetQuery("/app1/ifd/gps/subifd:{ulong=1}");
                    var latitudeQuery = Metadata.GetQuery("/app1/ifd/gps/subifd:{ulong=2}");

                    if (latitudeRefQuery != null && latitudeQuery != null)
                    {
                        string latitudeRef = latitudeRefQuery.ToString();
                        ulong[] latitude = latitudeQuery as ulong[];
                        return ConvertCoordinate(latitude, latitudeRef);
                    }
                    else
                    {
                        return null;
                    }

                }
                catch
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    Metadata.SetQuery("/app1/ifd/gps/subifd:{ulong=2}", ConvertCoordinate(value.Value));

                    byte[] gpsVersionNumbers = new byte[4];
                    gpsVersionNumbers[0] = 0;
                    gpsVersionNumbers[1] = 0;
                    gpsVersionNumbers[2] = 2;
                    gpsVersionNumbers[3] = 2;
                    Metadata.SetQuery("/app1/ifd/gps/", gpsVersionNumbers);

                    string northOrSouth = (value.Value >= 0) ? "N" : "S";
                    Metadata.SetQuery("/app1/ifd/gps/subifd:{ulong=1}", northOrSouth);
                }
                else
                {
                    if (Metadata.GetQuery("/app1/ifd/gps/subifd:{ulong=1}") != null)
                    {
                        Metadata.RemoveQuery("/app1/ifd/gps/subifd:{ulong=1}");
                    }

                    if (Metadata.GetQuery("/app1/ifd/gps/subifd:{ulong=2}") != null)
                    {
                        Metadata.RemoveQuery("/app1/ifd/gps/subifd:{ulong=2}");
                    }
                }
            }
        }

        public double? Longitude
        {
            get
            {
                try
                {
                    var longitudeRefQuery = Metadata.GetQuery("/app1/ifd/gps/subifd:{ulong=3}");
                    var longitudeQuery = Metadata.GetQuery("/app1/ifd/gps/subifd:{ulong=4}");

                    if (longitudeRefQuery != null && longitudeQuery != null)
                    {
                        string longitudeRef = longitudeRefQuery.ToString();
                        ulong[] longitude = longitudeQuery as ulong[];
                        return ConvertCoordinate(longitude, longitudeRef);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }

            }
            set
            {
                if (value.HasValue)
                {
                    Metadata.SetQuery("/app1/ifd/gps/subifd:{ulong=4}", ConvertCoordinate(value.Value));

                    byte[] gpsVersionNumbers = new byte[4];
                    gpsVersionNumbers[0] = 0;
                    gpsVersionNumbers[1] = 0;
                    gpsVersionNumbers[2] = 2;
                    gpsVersionNumbers[3] = 2;
                    Metadata.SetQuery("/app1/ifd/gps/", gpsVersionNumbers);

                    string eastOrWest = (value.Value >= 0) ? "E" : "W";
                    Metadata.SetQuery("/app1/ifd/Gps/subifd:{ulong=3}", eastOrWest);
                }
                else
                {
                    if (Metadata.GetQuery("/app1/ifd/Gps/subifd:{ulong=3}") != null)
                    {
                        Metadata.RemoveQuery("/app1/ifd/Gps/subifd:{ulong=3}");
                    }

                    if (Metadata.GetQuery("/app1/ifd/gps/subifd:{ulong=4}") != null)
                    {
                        Metadata.RemoveQuery("/app1/ifd/gps/subifd:{ulong=4}");
                    }
                }
            }
        }

        private double ConvertCoordinate(ulong[] coordinates, string gpsRef)
        {
            if (coordinates == null)
                return 0;

            int degrees;
            int minutes;
            double seconds;

            degrees = (int)splitLongAndDivide(coordinates[0]);
            minutes = (int)splitLongAndDivide(coordinates[1]);
            seconds = splitLongAndDivide(coordinates[2]);

            double coordinate = (double)degrees + (minutes / 60.0) + (seconds / 3600.0);

            double roundedCoordinate = Math.Floor((double)(coordinate * 1000000.0)) / 1000000.0;
            if (gpsRef == "S" || gpsRef == "W")
                roundedCoordinate = 0 - roundedCoordinate;

            return roundedCoordinate;
        }

        private double splitLongAndDivide(ulong number)
        {
            byte[] bytes = BitConverter.GetBytes(number);
            int int1 = BitConverter.ToInt32(bytes, 0);
            int int2 = BitConverter.ToInt32(bytes, 4);
            return ((double)int1 / (double)int2);
        }

        private ulong[] ConvertCoordinate(double coordinate)
        {
            //capture as many decimal places as possible for the degrees
            const uint _DegreesDenominator = 1000000;
            const double _SecondsRoundingFactor = 1000.0;

            ulong[] newStyleCoordinates = new ulong[3];

            // Make sure coordinate is positive.
            coordinate = Math.Abs(coordinate);

            double degrees = Math.Floor(coordinate);

            coordinate -= degrees;

            double minutes = Math.Floor(coordinate * 60.0);

            coordinate -= (minutes / 60.0);

            double seconds = Math.Round(coordinate * 3600.0 * _SecondsRoundingFactor) / _SecondsRoundingFactor;

            newStyleCoordinates[0] = EncodeExifCoordinatePart(degrees, 1);
            newStyleCoordinates[1] = EncodeExifCoordinatePart(minutes, 1);
            newStyleCoordinates[2] = EncodeExifCoordinatePart(seconds, _DegreesDenominator);

            return newStyleCoordinates;
        }

        private ulong EncodeExifCoordinatePart(double coordinatePart, uint denominator)
        {
            byte[] coordinatePartBytes = new byte[8];

            uint numerator = (uint)(coordinatePart * denominator);

            byte[] numeratorBytes = BitConverter.GetBytes(numerator);
            byte[] denominatorBytes = BitConverter.GetBytes(denominator);

            numeratorBytes.CopyTo(coordinatePartBytes, 0);
            denominatorBytes.CopyTo(coordinatePartBytes, 4);

            ulong encodedCoordinatePart = BitConverter.ToUInt64(coordinatePartBytes, 0);

            return encodedCoordinatePart;
        }

        public void Save()
        {
            SaveAs(path);
        }

        public void SaveWithCaption(string caption, DateTime? lastWrite = null)
        {
            string fileName = Path.GetFileNameWithoutExtension(caption) + Path.GetExtension(this.path);
            var path = Path.GetDirectoryName(this.path) + "\\" + fileName;
            SaveAs(path, lastWrite);
        }

        public void SaveAs(string path, DateTime? lastWrite = null)
        {
            if (!lastWrite.HasValue)
            {
                lastWrite = FileInfo.LastWriteTime;
            }

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(frame, frame.Thumbnail, Metadata, frame.ColorContexts));
            using (Stream stream = File.Open(path, FileMode.Create, FileAccess.ReadWrite))
            {
                encoder.Save(stream);
            }

            File.SetLastWriteTime(path, lastWrite.Value);

            if (this.path.ToLower() != path.ToLower())
            {
                File.Delete(this.path);
            }
        }

        private BitmapFrame getBitmapFrame(string path)
        {
            BitmapDecoder decoder = null;
            using (Stream stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                decoder = new JpegBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            }
            return decoder.Frames[0];
        }
    }
}
