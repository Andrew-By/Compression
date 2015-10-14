using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
    public class ByteStats : INotifyPropertyChanged
    {
        private int _total = 0;
        public int Total
        {
            get { return _total; }
            set
            {
                if (value != _total)
                {
                    _total = value;
                    NotifyPropertyChanged("Total");
                }
            }
        }

        private ObservableCollection<ByteStat> _bytes = new ObservableCollection<ByteStat>();
        public ObservableCollection<ByteStat> Bytes
        {
            get { return _bytes; }
        }

        public void Add(Byte b)
        {
            Total++;
            var oldByte = _bytes.FirstOrDefault(x => x.Byte == b);
            if (oldByte == null)
                _bytes.Add(new ByteStat(b));
            else
                oldByte.Count++;
        }

        public void UpdateFrequency()
        {
            foreach (var b in _bytes)
                b.Frequency = (double)b.Count / _total;
            var sorted = _bytes.OrderByDescending(x => x.Frequency).ToList();
            _bytes.Clear();
            foreach (var b in sorted)
                _bytes.Add(b);
        }

        private void _span(List<ByteStat> stats, string code, bool isFirst)
        {
            code += (isFirst) ? "0" : "1";
            if (stats.Count < 2)
                stats[0].Code = code.Substring(1);
            else
            {
                double totalFreq = stats.Sum(x => x.Frequency);
                double freq = 0;
                int i;
                for (i = 0; i < stats.Count; i++)
                {
                    freq += stats[i].Frequency;
                    if (freq >= totalFreq / 2)
                        break;
                }

                _span(stats.Take(i + 1).ToList(), code, true);
                _span(stats.Skip(i + 1).ToList(), code, false);
            }
        }

        public void Span()
        {
            _span(_bytes.ToList(), String.Empty, true);
        }

        public void Clear()
        {
            _total = 0;
            _bytes.Clear();
        }

        #region INotify
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }

    public class ByteStat : INotifyPropertyChanged
    {
        public ByteStat(byte b)
        {
            this.Byte = b;
            this.Count = 1;
        }

        private byte _byte;
        public byte Byte
        {
            get { return _byte; }
            set
            {
                if (value != _byte)
                {
                    _byte = value;
                    NotifyPropertyChanged("Byte");
                }
            }
        }

        public String Hex
        {
            get { return _byte.ToString("X2"); }
        }

        private int _count;
        public int Count
        {
            get { return _count; }
            set
            {
                if (value != _count)
                {
                    _count = value;
                    NotifyPropertyChanged("Count");
                }
            }
        }

        private double _frequency;
        public double Frequency
        {
            get { return _frequency; }
            set
            {
                if (value != _frequency)
                {
                    _frequency = value;
                    NotifyPropertyChanged("Frequency");
                }
            }
        }

        private String _code;
        public String Code
        {
            get { return _code; }
            set
            {
                if (value != _code)
                {
                    _code = value;
                    NotifyPropertyChanged("Code");
                }
            }
        }

        public override string ToString()
        {
            return Byte.ToString();
        }

        #region INotify
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
