using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UmlDiagrams
{
	/// <summary>
	/// Interaction logic for SequenceDiagram.xaml
	/// </summary>
	public partial class SequenceDiagram : UserControl
	{
		public SequenceDiagram()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Gets or sets the style to use when drawing the sequence diagram.
		/// </summary>
		public bool IsHandDrawnStyle
		{
			get { return (bool)GetValue(IsHandDrawnStyleProperty); }
			set { SetValue(IsHandDrawnStyleProperty, value); }
		}

		/// <summary>
		/// Gets or sets the foreground of an actor textbox.
		/// </summary>
		public Brush ActorForeground
		{
			get { return (Brush)GetValue(ActorForegroundProperty); }
			set { SetValue(ActorForegroundProperty, value); }
		}

		/// <summary>
		/// Gets or sets the background of an actor textbox.
		/// </summary>
		public Brush ActorBackground
		{
			get { return (Brush)GetValue(ActorBackgroundProperty); }
			set { SetValue(ActorBackgroundProperty, value); }
		}

		/// <summary>
		/// Gets or sets the foreground of a signal.
		/// </summary>
		public Brush SignalForeground
		{
			get { return (Brush)GetValue(SignalForegroundProperty); }
			set { SetValue(SignalForegroundProperty, value); }
		}

		/// <summary>
		/// Gets or sets the foreground of a note textbox.
		/// </summary>
		public Brush NoteForeground
		{
			get { return (Brush)GetValue(NoteForegroundProperty); }
			set { SetValue(NoteForegroundProperty, value); }
		}

		/// <summary>
		/// Gets or sets the background of a note text box.
		/// </summary>
		public Brush NoteBackground
		{
			get { return (Brush)GetValue(NoteBackgroundProperty); }
			set { SetValue(NoteBackgroundProperty, value); }
		}

		public static readonly DependencyProperty IsHandDrawnStyleProperty =
			DependencyProperty.Register(nameof(IsHandDrawnStyle), typeof(bool), typeof(SequenceDiagram),
				new FrameworkPropertyMetadata(false,
					FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty ActorForegroundProperty =
			DependencyProperty.Register(nameof(ActorForeground), typeof(Brush), typeof(SequenceDiagram),
				new FrameworkPropertyMetadata(Brushes.Black,
					FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty ActorBackgroundProperty =
			DependencyProperty.Register(nameof(ActorBackground), typeof(Brush), typeof(SequenceDiagram),
				new FrameworkPropertyMetadata(Brushes.White,
					FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty SignalForegroundProperty =
			DependencyProperty.Register(nameof(SignalForeground), typeof(Brush), typeof(SequenceDiagram),
				new FrameworkPropertyMetadata(Brushes.Black,
					FrameworkPropertyMetadataOptions.AffectsRender));

		private static readonly DependencyProperty NoteForegroundProperty =
			DependencyProperty.Register(nameof(NoteForeground), typeof(Brush), typeof(SequenceDiagram),
				new FrameworkPropertyMetadata(Brushes.Black,
					FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty NoteBackgroundProperty =
			DependencyProperty.Register(nameof(NoteBackground), typeof(Brush), typeof(SequenceDiagram),
				new FrameworkPropertyMetadata(Brushes.White,
					FrameworkPropertyMetadataOptions.AffectsRender));
	}
}
