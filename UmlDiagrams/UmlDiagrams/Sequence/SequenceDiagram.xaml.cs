using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
		#region Members

		private sealed class ActorVisualElement
		{
			public FormattedText Text;
			public Rect TextBB;
			public double?[] Distances;
			public double OffsetX;
		}

		private abstract class AnnotationVisualElement
		{
			public FormattedText Text;
			public Rect TextBB;
			public double OffsetY;
		}

		private sealed class SignalVisualElement : AnnotationVisualElement
		{
			public bool IsSelfSignal;
			public ActorVisualElement ActorA;
			public ActorVisualElement ActorB;
			public SequenceLineType LineType;
			public SequenceArrowType ArrowType;
		}

		private sealed class NoteVisualElement : AnnotationVisualElement
		{
			public SequenceNotePlacement Placement;
			public ActorVisualElement[] Actors;
			public string Message;
		}

		private static readonly Thickness s_actorMargin = new Thickness(10); // Margin around an actor
		private static readonly Thickness s_actorPadding = new Thickness(10, 5, 10, 5); // Padding inside an actor
		private static readonly Thickness s_signalMargin = new Thickness(5); // Margin around an annotation
		private static readonly Thickness s_signalPadding = new Thickness(5); // Padding inside an annotation
		private static readonly Thickness s_noteMargin = new Thickness(10); // Margin around a note
		private static readonly Thickness s_notePadding = new Thickness(5); // Padding inside a note
		private static readonly Thickness s_titleMargin = new Thickness(10); // Margin around a title
		private static readonly Thickness s_titlePadding = new Thickness(5); // Padding inside a title
		private static readonly Thickness s_noteOverlap = new Thickness(10); // Overlap when using a "note over A,B"
		private const double s_selfSignalWidth = 20.0; // How far out to place a self-signal
		private DrawingContext m_scaffold;
		private readonly bool m_showScaffold = false;

		private Typeface m_typeface;
		private List<ActorVisualElement> m_actorVisualElements;
		private List<SignalVisualElement> m_signalVisualElements;
		private List<NoteVisualElement> m_noteVisualElements;

		// All actor shapes share this width and height
		private double m_actorsWidth;
		private double m_actorsHeight;

		// Indicates the total bottom edge of all annotations
		private double m_annotationsBottom;

		#endregion

		#region Constructors

		static SequenceDiagram()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SequenceDiagram),
				new FrameworkPropertyMetadata(typeof(SequenceDiagram)));
		}

		public SequenceDiagram()
		{
			InitializeComponent();

			m_arrange = new Lazy<Size?>(ArrangeVisualElements);
		}

		#endregion

		#region Dependency Properties

		public static readonly DependencyProperty ViewModelProperty =
			DependencyProperty.Register(nameof(ViewModel), typeof(SequenceDiagramViewModel), typeof(SequenceDiagram),
				new FrameworkPropertyMetadata(default(SequenceDiagramViewModel),
					FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

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

		public static readonly DependencyProperty NoteForegroundProperty =
			DependencyProperty.Register(nameof(NoteForeground), typeof(Brush), typeof(SequenceDiagram),
				new FrameworkPropertyMetadata(Brushes.Black,
					FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty NoteBackgroundProperty =
			DependencyProperty.Register(nameof(NoteBackground), typeof(Brush), typeof(SequenceDiagram),
				new FrameworkPropertyMetadata(Brushes.White,
					FrameworkPropertyMetadataOptions.AffectsRender));

		public SequenceDiagramViewModel ViewModel
		{
			get { return (SequenceDiagramViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}

		public bool IsHandDrawnStyle
		{
			get { return (bool)GetValue(IsHandDrawnStyleProperty); }
			set { SetValue(IsHandDrawnStyleProperty, value); }
		}

		public Brush ActorForeground
		{
			get { return (Brush)GetValue(ActorForegroundProperty); }
			set { SetValue(ActorForegroundProperty, value); }
		}

		public Brush ActorBackground
		{
			get { return (Brush)GetValue(ActorBackgroundProperty); }
			set { SetValue(ActorBackgroundProperty, value); }
		}

		public Brush SignalForeground
		{
			get { return (Brush)GetValue(SignalForegroundProperty); }
			set { SetValue(SignalForegroundProperty, value); }
		}

		public Brush NoteForeground
		{
			get { return (Brush)GetValue(NoteForegroundProperty); }
			set { SetValue(NoteForegroundProperty, value); }
		}

		public Brush NoteBackground
		{
			get { return (Brush)GetValue(NoteBackgroundProperty); }
			set { SetValue(NoteBackgroundProperty, value); }
		}

		#endregion

		#region Methods

		private Lazy<Size?> m_arrange;

		protected override Size MeasureOverride(Size constraint)
		{
			Size size = base.MeasureOverride(constraint);

			m_arrange = new Lazy<Size?>(ArrangeVisualElements);

			return m_arrange.Value ?? size;
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			Size size = base.ArrangeOverride(arrangeBounds);

			return m_arrange.Value ?? size;
		}

		private Size? ArrangeVisualElements()
		{
			// Early out if there sequence diagram view model available
			var vm = DataContext as SequenceDiagramViewModel;
			if (vm == null)
				return null;

			// Reset visual elements
			var actors = vm.Actors;
			var annotations = vm.Annotations;
			m_actorVisualElements = new List<ActorVisualElement>(actors.Count);
			m_signalVisualElements = new List<SignalVisualElement>();
			m_noteVisualElements = new List<NoteVisualElement>();
			m_actorsWidth = 0;
			m_actorsHeight = 0;
			m_annotationsBottom = 0;
			double offsetY = Margin.Top;

			// Capture typeface which is used to measure various textboxes
			m_typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);

			// Layout actors
			foreach (var actor in actors)
			{
				var text = GetFormattedText(actor.Name, ActorForeground);
				var textBB = GetTextBB(text, s_actorMargin, s_actorPadding);

				var actorVisualElement = new ActorVisualElement
				{
					Text = text,
					TextBB = textBB,
					Distances = new double?[actors.Count]
				};
				m_actorVisualElements.Add(actorVisualElement);

				m_actorsWidth = Math.Max(textBB.Width, m_actorsWidth);
				m_actorsHeight = Math.Max(textBB.Height, m_actorsHeight);
			}

			// Position at bottom edge of top actor text boxes
			offsetY += m_actorsHeight;

			// Layout all signal types
			foreach (var annotation in annotations)
			{
				double extraWidth = 0;
				bool skipFinalSignalLayout = false;

				// Indexes of the left and right actors involved
				int a = 0, b = 0;

				// Visual element
				AnnotationVisualElement annotationVisualElement = null;

				// Handle signals
				var signal = annotation as SignalViewModel;
				if (signal != null)
				{
					var text = GetFormattedText(annotation.Message, SignalForeground);
					var textBB = GetTextBB(text, s_signalMargin, s_signalPadding);

					var signalVisualElement = new SignalVisualElement
					{
						Text = text,
						TextBB = textBB,
						OffsetY = offsetY,
						IsSelfSignal = signal.IsSelf(),
						ActorA = m_actorVisualElements[signal.ActorA.Index],
						ActorB = m_actorVisualElements[signal.ActorB.Index],
						LineType = signal.LineType,
						ArrowType = signal.ArrowType,
					};
					annotationVisualElement = signalVisualElement;
					m_signalVisualElements.Add(signalVisualElement);

					if (signal.IsSelf())
					{
						// Self-signals need a minimum height
						a = signal.ActorA.Index;
						b = a + 1;

						// Self-signals take up extra horizontal space
						extraWidth += s_selfSignalWidth;
					}
					else
					{
						a = Math.Min(signal.ActorA.Index, signal.ActorB.Index);
						b = Math.Max(signal.ActorA.Index, signal.ActorB.Index);
					}
				}

				// Handle notes
				var note = annotation as NoteViewModel;
				if (note != null)
				{
					var text = GetFormattedText(annotation.Message, NoteForeground);
					var textBB = GetTextBB(text, s_noteMargin, s_notePadding);

					var noteVisualElement = new NoteVisualElement
					{
						Text = text,
						TextBB = textBB,
						OffsetY = offsetY,
						Placement = note.Placement,
						Message = note.Message,
						Actors = note.Actors.Select(actor => m_actorVisualElements[actor.Index]).ToArray(),
					};
					annotationVisualElement = noteVisualElement;
					m_noteVisualElements.Add(noteVisualElement);

					if (note.Placement == SequenceNotePlacement.LeftOf)
					{
						b = note.Actors[0].Index;
						a = b - 1;
					}
					else if (note.Placement == SequenceNotePlacement.RightOf)
					{
						a = note.Actors[0].Index;
						b = a + 1;
					}
					else if (note.Placement == SequenceNotePlacement.Over)
					{
						if (note.Actors.Length > 1)
						{
							// Over multiple actors
							a = Math.Min(note.Actors[0].Index, note.Actors[1].Index);
							b = Math.Max(note.Actors[0].Index, note.Actors[1].Index);

							// We don't need our padding, and we want to overlap
							extraWidth = -(s_noteMargin.Left + s_noteMargin.Right + s_noteOverlap.Left + s_noteOverlap.Right);
						}
						else
						{
							// Over single actor
							a = note.Actors[0].Index;
							ActorEnsureDistance(a - 1, a, noteVisualElement.TextBB.Width / 2);
							ActorEnsureDistance(a, a + 1, noteVisualElement.TextBB.Width / 2);

							// Skip calculation of the signal height in this case
							skipFinalSignalLayout = true;
						}
					}
				}

				// Ensure proper spacing between actors
				if (!skipFinalSignalLayout)
					ActorEnsureDistance(a, b, annotationVisualElement.TextBB.Width + extraWidth);

				// Adjust offset by by annotiation height
				offsetY += annotationVisualElement.TextBB.Height;
			}
			m_annotationsBottom = offsetY;

			// Compute the overall size of the diagram
			double actorsX = 0;
			foreach (var actor in m_actorVisualElements)
			{
				actor.OffsetX = Math.Max(actorsX, actor.OffsetX);

				// This requires that we enumerate distances in order
				for (int i = 0; i < actor.Distances.Length; ++i)
				{
					double? distance = actor.Distances[i];

					// Only process well-defined distances
					if (distance.HasValue)
					{
						ActorVisualElement b = m_actorVisualElements[i];
						double d = Math.Max(distance.Value, Math.Max(m_actorsWidth / 2, b.TextBB.Width / 2));
						b.OffsetX = Math.Max(b.OffsetX, actor.OffsetX + m_actorsWidth / 2 + d - b.TextBB.Width / 2);
					}
				}

				actorsX = actor.OffsetX + m_actorsWidth;
			}

			// Determine total size
			Size size = new Size();
			size.Width = Math.Max(actorsX, size.Width) + Margin.Left + Margin.Right;
			size.Height = Margin.Top + Margin.Bottom + m_annotationsBottom + m_actorsHeight;
			return size;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			if (m_showScaffold)
				m_scaffold = drawingContext;

			using (IDrawingCanvas drawingCanvas = CreateDrawingCanvas(drawingContext))
			{
				// Draw our sequence
				DrawActors(drawingCanvas);
				DrawSignals(drawingCanvas);
				DrawNotes(drawingCanvas);
			}
		}

		private IDrawingCanvas CreateDrawingCanvas(DrawingContext drawingContext)
		{
			if (IsHandDrawnStyle)
				return new HandDrawnCanvas(drawingContext, Foreground);
			else
				return new DefaultDrawingCanvas(drawingContext, Foreground);
		}

		private void DrawActors(IDrawingCanvas drawingCanvas)
		{
			if (m_actorVisualElements == null)
				return;

			foreach (var actor in m_actorVisualElements)
			{
				// Vertical line
				double x = GetCenterX(actor) + actor.OffsetX;
				drawingCanvas.DrawLine(
					x, m_actorsHeight / 2,
					x, m_annotationsBottom + m_actorsHeight / 2,
					SequenceLineType.Solid,
					SequenceArrowType.None
				);

				// Top box
				DrawActor(drawingCanvas, actor, 0);

				// Bottom box
				DrawActor(drawingCanvas, actor, m_annotationsBottom);
			}

			// Scaffold lines
			if (m_scaffold != null)
			{
				m_scaffold.DrawLine(new Pen(Brushes.Purple, 1), new Point(0, s_actorMargin.Top), new Point(200, s_actorMargin.Top));
				m_scaffold.DrawLine(new Pen(Brushes.MediumPurple, 1), new Point(0, m_actorsHeight - s_actorMargin.Bottom), new Point(200, m_actorsHeight - s_actorMargin.Bottom));

				m_scaffold.DrawLine(new Pen(Brushes.Purple, 1), new Point(0, m_actorsHeight + m_annotationsBottom + s_actorMargin.Top), new Point(200, m_actorsHeight + m_annotationsBottom + s_actorMargin.Top));
				m_scaffold.DrawLine(new Pen(Brushes.MediumPurple, 1), new Point(0, 2 * m_actorsHeight + m_annotationsBottom - s_actorMargin.Bottom), new Point(200, 2 * m_actorsHeight + m_annotationsBottom - s_actorMargin.Bottom));
			}
		}

		private void DrawActor(IDrawingCanvas drawingCanvas, ActorVisualElement actor, double offsetY)
		{
			Rect box = new Rect(actor.OffsetX, offsetY, m_actorsWidth, m_actorsHeight);
			DrawTextBox(drawingCanvas, box, actor.Text, ActorBackground, s_actorMargin, s_actorPadding, true);
		}

		private void DrawSignals(IDrawingCanvas drawingCanvas)
		{
			if (m_signalVisualElements == null)
				return;

			foreach (var signal in m_signalVisualElements)
			{
				if (signal.IsSelfSignal)
					DrawSelfSignal(drawingCanvas, signal);
				else
					DrawSignal(drawingCanvas, signal);
			}
		}

		private void DrawSelfSignal(IDrawingCanvas drawingCanvas, SignalVisualElement signal)
		{
			// Actor line position
			double aX = GetCenterX(signal.ActorA) + signal.ActorA.OffsetX;

			// Left and right edges of self-signal line
			double x1 = aX;
			double x2 = x1 + s_selfSignalWidth;

			// Top and bottom of self-signal line
			double y1 = signal.OffsetY + s_signalMargin.Top;
			double y2 = signal.OffsetY + signal.TextBB.Height - s_signalMargin.Bottom;

			// Position of signal text
			double x = x2 + s_signalPadding.Left;
			double y = y1 + s_signalPadding.Top;

			// Draw the text so that its left edge is along the vertical segment of the self-signal
			signal.Text.TextAlignment = TextAlignment.Left;
			drawingCanvas.DrawText(x, y, signal.Text);

			// Draw three lines, the last one with an arrow
			drawingCanvas.DrawLine(x1, y1, x2, y1, signal.LineType, SequenceArrowType.None);
			drawingCanvas.DrawLine(x2, y1, x2, y2, signal.LineType, SequenceArrowType.None);
			drawingCanvas.DrawLine(x2, y2, x1, y2, signal.LineType, signal.ArrowType);
		}

		private void DrawSignal(IDrawingCanvas drawingCanvas, SignalVisualElement signal)
		{
			double aX = GetCenterX(signal.ActorA) + signal.ActorA.OffsetX;
			double bX = GetCenterX(signal.ActorB) + signal.ActorB.OffsetX;

			// Mid-point between actors
			double x = (bX + aX) / 2;
			double y = signal.OffsetY + s_signalMargin.Top + s_signalPadding.Top;

			// Scaffold lines
			if (m_scaffold != null)
			{
				m_scaffold.DrawLine(new Pen(Brushes.Red, 1), new Point(0, signal.OffsetY), new Point(x, signal.OffsetY));
				m_scaffold.DrawLine(new Pen(Brushes.Green, 1), new Point(0, signal.OffsetY + s_signalMargin.Top), new Point(x, signal.OffsetY + s_signalMargin.Top));
				m_scaffold.DrawLine(new Pen(Brushes.LightGray, 1), new Point(0, y), new Point(x, y));
				double y2 = signal.OffsetY + signal.TextBB.Height - s_signalMargin.Bottom - s_signalPadding.Bottom;
				m_scaffold.DrawLine(new Pen(Brushes.LightGray, 1), new Point(10, y2), new Point(x, y2));
				m_scaffold.DrawLine(new Pen(Brushes.Green, 1), new Point(10, y2 + s_signalPadding.Bottom), new Point(x, y2 + s_signalPadding.Bottom));
				m_scaffold.DrawLine(new Pen(Brushes.Red, 1), new Point(10, y2 + s_signalPadding.Bottom + s_signalMargin.Bottom), new Point(x, y2 + s_signalPadding.Bottom + s_signalMargin.Bottom));
			}

			// Draw the text in the middle of the signal
			signal.Text.TextAlignment = TextAlignment.Center;
			drawingCanvas.DrawText(x, y, signal.Text);

			// Draw the line along the bottom of the signal
			y = signal.OffsetY + signal.TextBB.Height - s_signalMargin.Bottom;
			drawingCanvas.DrawLine(aX, y, bX, y, signal.LineType, signal.ArrowType);
		}

		private void DrawNotes(IDrawingCanvas drawingCanvas)
		{
			if (m_noteVisualElements == null)
				return;

			foreach (var note in m_noteVisualElements)
			{
				DrawNote(drawingCanvas, note);
			}
		}

		private void DrawNote(IDrawingCanvas drawingCanvas, NoteVisualElement note)
		{
			var actorA = note.Actors[0];
			double aX = GetCenterX(actorA) + actorA.OffsetX;

			double x = 0;
			double width = note.TextBB.Width;

			switch (note.Placement)
			{
				case SequenceNotePlacement.RightOf:
					x = aX;
					break;
				case SequenceNotePlacement.LeftOf:
					x = aX - note.TextBB.Width;
					break;
				case SequenceNotePlacement.Over:
					if (note.Actors.Length > 1)
					{
						double bX = GetCenterX(note.Actors[1]) + note.Actors[1].OffsetX;
						double overlapLeft = s_noteOverlap.Left + s_noteMargin.Left;
						double overlapRight = s_noteOverlap.Right + s_noteMargin.Right;
						x = Math.Min(aX, bX) - overlapLeft;
						width = Math.Max(aX, bX) + overlapRight - x;
					}
					else
					{
						x = aX - note.TextBB.Width / 2;
					}
					break;
				default:
					throw new NotSupportedException($"Unhandled note placement: {note.Placement}");
			}

			// Draw note
			Rect box = new Rect(x, note.OffsetY, width, note.TextBB.Height);
			DrawTextBox(drawingCanvas, box, note.Text, NoteBackground, s_noteMargin, s_notePadding, false);
		}

		private void DrawTextBox(IDrawingCanvas drawingCanvas, Rect box, FormattedText text, Brush brush, Thickness margin, Thickness padding, bool alignCenter)
		{
			var x = box.X + margin.Left;
			var y = box.Y + margin.Top;
			var w = box.Width - margin.Left - margin.Right;
			var h = box.Height - margin.Top - margin.Bottom;

			// Draw inner box
			Rect rect = new Rect(x, y, w, h);
			drawingCanvas.DrawRectangle(brush, new Pen(Brushes.Transparent, 1), rect);

			// Draw text (in the center)
			if (alignCenter)
			{
				x = GetCenterX(box);
				y += padding.Top;
				text.TextAlignment = TextAlignment.Center;
			}
			else
			{
				x += padding.Left;
				y += padding.Top;
				text.TextAlignment = TextAlignment.Left;
			}

			drawingCanvas.DrawText(x, y, text);
		}

		private FormattedText GetFormattedText(string text, Brush brush)
		{
			var pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
			return new FormattedText(
				text,
				CultureInfo.CurrentUICulture,
				FlowDirection,
				m_typeface,
				FontSize,
				brush,
				pixelsPerDip);
		}

		private void ActorEnsureDistance(int a, int b, double d)
		{
			if (a >= b)
				throw new ArgumentException($"{nameof(a)} must be less than {nameof(b)}");

			var actors = m_actorVisualElements;
			if (a < 0)
			{
				// Ensure b has left margin
				var actorB = actors[b];
				actorB.OffsetX = Math.Max(d - actorB.TextBB.Width / 2, actorB.OffsetX);
			}
			else if (b >= actors.Count)
			{
				// Ensure a has right margin
				var actorA = actors[a];
			}
			else
			{
				var actorA = actors[a];
				actorA.Distances[b] = Math.Max(d, actorA.Distances[b] ?? 0);
			}
		}

		private double GetCenterX(ActorVisualElement actorVisualElement)
		{
			return actorVisualElement.TextBB.X + m_actorsWidth / 2;
		}

		private static double GetCenterX(Rect box)
		{
			return box.X + box.Width / 2;
		}

		private static double GetCenterY(Rect box)
		{
			return box.Y + box.Height / 2;
		}

		private static Rect GetTextBB(FormattedText text, Thickness margin, Thickness padding)
		{
			return new Rect(0, 0,
				text.Width + margin.Left + margin.Right + padding.Left + padding.Right,
				text.Height + margin.Top + margin.Bottom + padding.Top + padding.Bottom);
		}

		#endregion
	}
}
