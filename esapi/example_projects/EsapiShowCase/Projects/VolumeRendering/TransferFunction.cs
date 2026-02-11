using System;
using System.Xml;
using System.Collections.Generic;
using System.Windows.Media;
using System.ComponentModel;

namespace Sample3DViewer
{
  public class TransferFunctionPoint : INotifyPropertyChanged
  {
    public int HU 
    {
      get
      {
        return (int)((m_position * 65535) - 1000);
      }
      set
      {
        if (value >= -1000 && value < 65535)
        {
          m_position = ((float)(value + 1000) / 65535);
          OnPropertyChanged();
        }
      }
    }

    public byte Red 
    {
      get { return m_color.R; }
      set
      {
        m_color.R = value;
        OnPropertyChanged();
      }
    }
    public byte Green
    {
      get { return m_color.G; }
      set
      {
        m_color.G = value;
        OnPropertyChanged();
      }
    }
    public byte Blue
    {
      get { return m_color.B; }
      set
      {
        m_color.B = value;
        OnPropertyChanged();
      }
    }
    public byte Alpha
    {
      get { return m_color.A; }
      set
      {
        m_color.A = value;
        OnPropertyChanged();
      }
    }

    public float m_position; //Position { get; set; }
    public Color m_color;    // { get; set; }

    private void OnPropertyChanged()
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(null));
    }
    public event PropertyChangedEventHandler PropertyChanged;
  }

    public class TransferFunction
    {
        public string FileName { get; private set; }

        public float[] Positions { get; set; }
        public float[] Colors { get; set; }

        public TransferFunction(string filename)
        {
            FileName = filename;
            Load(filename);
        }

        public void UpdateFromPoints()
        {
          if (m_points == null)
            return;

          int count = m_points.Count;
          Positions = new float[count];
          Colors = new float[count * 4];

          for (int i = 0; i < count; i++)
          {
            Positions[i] = m_points[i].m_position;
            Color col = m_points[i].m_color;
            float blue  = col.ScB;
            float green = col.ScG;
            float red   = col.ScR;
            float alpha = col.ScA;
            Colors[i * 4 + 0] = blue;
            Colors[i * 4 + 1] = green;
            Colors[i * 4 + 2] = red;
            Colors[i * 4 + 3] = alpha;
          }
        }
        List<TransferFunctionPoint> m_points = null;
        public List<TransferFunctionPoint> Points 
        { 
          get 
          {
            if (m_points == null)
            {
              m_points = new List<TransferFunctionPoint>();
              for (int i = 0; i < Positions.GetLength(0); i++)
              {
                float blue  = Colors[i * 4 + 0];
                float green = Colors[i * 4 + 1];
                float red   = Colors[i * 4 + 2];
                float alpha = Colors[i * 4 + 3];
                
                Color col = new Color();
                col.ScR = red;
                col.ScG = green;
                col.ScB = blue;
                col.ScA = alpha;
                TransferFunctionPoint point = new TransferFunctionPoint();
                point.m_color = col; 
                point.m_position = Positions[i];
                m_points.Add(point);
              }
            }
            return m_points; 
          } 
        }

        public void Load(string path)
        {
            if (path == string.Empty)
                return;
            m_points = null;
            XmlDocument document = new XmlDocument();
            document.Load(path); // can throw

            XmlNode versionNode = document.SelectSingleNode(@"transfer/@version");
            if (versionNode == null)
            {
                versionNode = document.SelectSingleNode(@"TransferFunction/@Version");
                if (versionNode == null)
                    throw new Exception("Missing version attribute in root node.");
            }

            if (versionNode.Value == "1.2")
            {
                XmlNodeList keypointNodes = document.SelectNodes("/TransferFunction/KeyPoints/KeyPoint");
                Positions = new float[keypointNodes.Count];
                Colors = new float[keypointNodes.Count * 4];

                int counter = 0;
                foreach (XmlNode x in keypointNodes)
                {
                    // Any of these can throw
                    float position = float.Parse(x.SelectSingleNode("@Position").Value, System.Globalization.CultureInfo.InvariantCulture);
                    Positions[counter] = position;

                    float red = float.Parse(x.SelectSingleNode("@R").Value, System.Globalization.CultureInfo.InvariantCulture);
                    float green = float.Parse(x.SelectSingleNode("@G").Value, System.Globalization.CultureInfo.InvariantCulture);
                    float blue = float.Parse(x.SelectSingleNode("@B").Value, System.Globalization.CultureInfo.InvariantCulture);
                    float alpha = float.Parse(x.SelectSingleNode("@A").Value, System.Globalization.CultureInfo.InvariantCulture);

                    Colors[counter * 4 + 0] = blue;
                    Colors[counter * 4 + 1] = green;
                    Colors[counter * 4 + 2] = red;
                    Colors[counter * 4 + 3] = alpha;
                    counter++;
                }
            }
        }
    }    
}
