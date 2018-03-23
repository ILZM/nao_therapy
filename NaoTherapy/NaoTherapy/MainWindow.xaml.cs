using Main;
using Microsoft.Kinect;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;

namespace NaoTherapy
{
    public partial class MainWindow : Window
    {
        private Logic logic	= null;

		[ DllImport ( "kernel32.dll" ) ]
		public static extern bool AllocConsole ( );

        public MainWindow ( )
        {
			AllocConsole ( );
			
            this.InitializeComponent ( );
		}

        private void MainWindow_Loaded ( object sender, RoutedEventArgs e )
        {
			this.logic = new Logic ( this );
        }

        protected override void OnClosing ( CancelEventArgs e )
        {
            base.OnClosing ( e );

			System.Environment.Exit ( 0 );
		}

		public void setTitle ( string title )
		{
			this.Title = title;
		}

		public void DrawKinectSkeletons ( Skeleton [ ] kinectSkeletons )
		{
			this.painter.Draw ( kinectSkeletons );
		}
    }
}