using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace Vinco.Silverlight.Views
{
    public interface IUploadView
    {
        object DataContext { get; set; }
    }

    public partial class UploadView : UserControl, IUploadView
    {
        public UploadView()
        {
            InitializeComponent();
        }
    }
}