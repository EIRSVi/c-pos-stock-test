using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Stock_Room.Models;

namespace Stock_Room.UI
{
    /// <summary>
    /// Custom UI components for the inventory management system
    /// </summary>
    
    /// <summary>
    /// Modern styled button with FontAwesome icon support
    /// </summary>
    public class ModernButton : Button
    {
        private Color primaryColor = ResourceManager.Theme.Primary;
        private Color hoverColor = ResourceManager.Theme.PrimaryLight;
        private bool isHovered = false;
        
        public Color PrimaryColor
        {
            get => primaryColor;
            set { primaryColor = value; Invalidate(); }
        }
        
        public Color HoverColor
        {
            get => hoverColor;
            set { hoverColor = value; Invalidate(); }
        }

        public ModernButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                    ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = primaryColor;
            ForeColor = Color.White;
            Font = ResourceManager.Typography.ButtonText;
            Cursor = Cursors.Hand;
            Size = new Size(120, 40);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            BackColor = hoverColor;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            BackColor = primaryColor;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw rounded background
            using (var brush = new SolidBrush(isHovered ? hoverColor : primaryColor))
            {
                var rect = new Rectangle(0, 0, Width, Height);
                var path = GetRoundedRectPath(rect, 8);
                graphics.FillPath(brush, path);
            }

            // Draw text and image
            var textRect = ClientRectangle;
            if (Image != null)
            {
                var imageRect = new Rectangle(Padding.Left, (Height - Image.Height) / 2, Image.Width, Image.Height);
                graphics.DrawImage(Image, imageRect);
                textRect.X = imageRect.Right + 8;
                textRect.Width -= imageRect.Right + 8;
            }

            TextRenderer.DrawText(graphics, Text, Font, textRect, ForeColor, 
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// Modern styled DataGridView with enhanced appearance
    /// </summary>
    public class ModernDataGridView : DataGridView
    {
        public ModernDataGridView()
        {
            // Basic styling
            BackgroundColor = ResourceManager.Theme.Surface;
            BorderStyle = BorderStyle.None;
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            GridColor = ResourceManager.Theme.Border;
            
            // Header styling
            EnableHeadersVisualStyles = false;
            ColumnHeadersDefaultCellStyle.BackColor = ResourceManager.Theme.Secondary;
            ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            ColumnHeadersDefaultCellStyle.Font = ResourceManager.Typography.HeaderSmall;
            ColumnHeadersDefaultCellStyle.SelectionBackColor = ResourceManager.Theme.SecondaryDark;
            ColumnHeadersHeight = 45;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            
            // Row styling
            DefaultCellStyle.BackColor = ResourceManager.Theme.Surface;
            DefaultCellStyle.ForeColor = ResourceManager.Theme.TextPrimary;
            DefaultCellStyle.Font = ResourceManager.Typography.BodyMedium;
            DefaultCellStyle.SelectionBackColor = ResourceManager.Theme.PrimaryLight;
            DefaultCellStyle.SelectionForeColor = Color.White;
            RowTemplate.Height = 40;
            
            // Alternating row colors
            AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            
            // Selection settings
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            MultiSelect = false;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            ReadOnly = true;
            
            // Scrollbar styling
            ScrollBars = ScrollBars.Vertical;
            
            // Performance
            DoubleBuffered = true;
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Custom cell painting for better appearance
                e.Graphics.FillRectangle(new SolidBrush(e.CellStyle.BackColor), e.CellBounds);
                
                if (e.Value != null)
                {
                    TextRenderer.DrawText(e.Graphics, e.Value.ToString(), e.CellStyle.Font,
                        e.CellBounds, e.CellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                }
                
                e.Handled = true;
            }
            else
            {
                base.OnCellPainting(e);
            }
        }
    }

    /// <summary>
    /// Modern styled TextBox with placeholder support
    /// </summary>
    public class ModernTextBox : TextBox
    {
        private string placeholder = string.Empty;
        private Color placeholderColor = ResourceManager.Theme.TextMuted;
        private bool isPlaceholderActive = false;

        public string Placeholder
        {
            get => placeholder;
            set
            {
                placeholder = value;
                if (string.IsNullOrEmpty(Text))
                {
                    SetPlaceholder();
                }
            }
        }

        public Color PlaceholderColor
        {
            get => placeholderColor;
            set { placeholderColor = value; Invalidate(); }
        }

        public ModernTextBox()
        {
            BorderStyle = BorderStyle.FixedSingle;
            Font = ResourceManager.Typography.BodyMedium;
            Height = 35;
            Padding = new Padding(10, 8, 10, 8);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (isPlaceholderActive)
            {
                ClearPlaceholder();
            }
            BackColor = Color.White;
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            if (string.IsNullOrEmpty(Text))
            {
                SetPlaceholder();
            }
            BackColor = ResourceManager.Theme.Surface;
        }

        private void SetPlaceholder()
        {
            isPlaceholderActive = true;
            Text = placeholder;
            ForeColor = placeholderColor;
        }

        private void ClearPlaceholder()
        {
            isPlaceholderActive = false;
            Text = string.Empty;
            ForeColor = ResourceManager.Theme.TextPrimary;
        }

        public string GetValue()
        {
            return isPlaceholderActive ? string.Empty : Text;
        }

        public void SetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                SetPlaceholder();
            }
            else
            {
                isPlaceholderActive = false;
                Text = value;
                ForeColor = ResourceManager.Theme.TextPrimary;
            }
        }
    }

    /// <summary>
    /// Modern styled ComboBox
    /// </summary>
    public class ModernComboBox : ComboBox
    {
        public ModernComboBox()
        {
            FlatStyle = FlatStyle.Flat;
            BackColor = ResourceManager.Theme.Surface;
            ForeColor = ResourceManager.Theme.TextPrimary;
            Font = ResourceManager.Typography.BodyMedium;
            Height = 35;
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Custom dropdown arrow
            var arrowRect = new Rectangle(Width - 20, (Height - 10) / 2, 15, 10);
            using (var brush = new SolidBrush(ResourceManager.Theme.TextSecondary))
            {
                var points = new Point[]
                {
                    new Point(arrowRect.Left, arrowRect.Top),
                    new Point(arrowRect.Right, arrowRect.Top),
                    new Point(arrowRect.Left + arrowRect.Width / 2, arrowRect.Bottom)
                };
                e.Graphics.FillPolygon(brush, points);
            }
        }
    }

    /// <summary>
    /// Modern styled Panel with shadow effect
    /// </summary>
    public class ModernPanel : Panel
    {
        private bool hasShadow = true;
        private int shadowDepth = 4;
        private Color shadowColor = Color.FromArgb(20, 0, 0, 0);

        public bool HasShadow
        {
            get => hasShadow;
            set { hasShadow = value; Invalidate(); }
        }

        public int ShadowDepth
        {
            get => shadowDepth;
            set { shadowDepth = value; Invalidate(); }
        }

        public ModernPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                    ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            
            BackColor = ResourceManager.Theme.Surface;
            Padding = new Padding(20);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (hasShadow)
            {
                // Draw shadow
                using (var shadowBrush = new SolidBrush(shadowColor))
                {
                    for (int i = 1; i <= shadowDepth; i++)
                    {
                        var shadowRect = new Rectangle(i, i, Width - i, Height - i);
                        var shadowPath = GetRoundedRectPath(shadowRect, 8);
                        graphics.FillPath(shadowBrush, shadowPath);
                    }
                }
            }

            // Draw main panel
            using (var brush = new SolidBrush(BackColor))
            {
                var mainRect = new Rectangle(0, 0, Width - shadowDepth, Height - shadowDepth);
                var mainPath = GetRoundedRectPath(mainRect, 8);
                graphics.FillPath(brush, mainPath);
            }

            // Draw border
            using (var pen = new Pen(ResourceManager.Theme.Border, 1))
            {
                var borderRect = new Rectangle(0, 0, Width - shadowDepth - 1, Height - shadowDepth - 1);
                var borderPath = GetRoundedRectPath(borderRect, 8);
                graphics.DrawPath(pen, borderPath);
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// Status indicator control
    /// </summary>
    public class StatusIndicator : Control
    {
        private string status = "Active";
        private Color statusColor = ResourceManager.Theme.Success;

        public string Status
        {
            get => status;
            set
            {
                status = value;
                statusColor = GetStatusColor(value);
                Invalidate();
            }
        }

        public StatusIndicator()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                    ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            
            Size = new Size(80, 25);
            Font = ResourceManager.Typography.BodySmall;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw background
            using (var brush = new SolidBrush(Color.FromArgb(20, statusColor)))
            {
                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                graphics.FillRoundedRectangle(brush, rect, 12);
            }

            // Draw border
            using (var pen = new Pen(statusColor, 1))
            {
                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                graphics.DrawRoundedRectangle(pen, rect, 12);
            }

            // Draw text
            using (var brush = new SolidBrush(statusColor))
            {
                var textRect = ClientRectangle;
                TextRenderer.DrawText(graphics, status, Font, textRect, statusColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private Color GetStatusColor(string status)
        {
            return status.ToLower() switch
            {
                "active" => ResourceManager.Theme.Success,
                "inactive" => ResourceManager.Theme.TextMuted,
                "low stock" => ResourceManager.Theme.Warning,
                "out of stock" => ResourceManager.Theme.Danger,
                "discontinued" => ResourceManager.Theme.Danger,
                _ => ResourceManager.Theme.TextSecondary
            };
        }
    }

    /// <summary>
    /// Extension methods for Graphics class
    /// </summary>
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle rect, int radius)
        {
            using (var path = GetRoundedRectPath(rect, radius))
            {
                graphics.FillPath(brush, path);
            }
        }

        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle rect, int radius)
        {
            using (var path = GetRoundedRectPath(rect, radius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private static GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}