using System.Reflection;
using Svg;
using System.Drawing.Drawing2D;

namespace Stock_Room
{
    /// <summary>
    /// Comprehensive resource manager for UI components, icons, themes, and styling
    /// </summary>
    public static class ResourceManager
    {
        private static readonly string BaseResourcePath = "Resources";
        private static readonly string FontAwesomePath = @"X:\c\resources\icon\fontawesome-free-7.0.0-desktop";
        
        // Theme and styling constants
        public static class Theme
        {
            // Primary colors
            public static readonly Color Primary = Color.FromArgb(37, 99, 235);
            public static readonly Color PrimaryDark = Color.FromArgb(29, 78, 216);
            public static readonly Color PrimaryLight = Color.FromArgb(59, 130, 246);
            
            // Secondary colors
            public static readonly Color Secondary = Color.FromArgb(15, 23, 42);
            public static readonly Color SecondaryLight = Color.FromArgb(30, 41, 59);
            public static readonly Color SecondaryDark = Color.FromArgb(2, 6, 23);
            
            // Status colors
            public static readonly Color Success = Color.FromArgb(34, 197, 94);
            public static readonly Color Warning = Color.FromArgb(245, 158, 11);
            public static readonly Color Danger = Color.FromArgb(239, 68, 68);
            public static readonly Color Info = Color.FromArgb(59, 130, 246);
            
            // Neutral colors
            public static readonly Color Background = Color.FromArgb(248, 250, 252);
            public static readonly Color Surface = Color.White;
            public static readonly Color Border = Color.FromArgb(229, 231, 235);
            public static readonly Color TextPrimary = Color.FromArgb(15, 23, 42);
            public static readonly Color TextSecondary = Color.FromArgb(71, 85, 105);
            public static readonly Color TextMuted = Color.FromArgb(148, 163, 184);
            
            // Button colors
            public static readonly Color ButtonText = Color.White;
            public static readonly Color ButtonDisabled = Color.FromArgb(156, 163, 175);
            
            // Sidebar colors
            public static readonly Color SidebarActive = Color.FromArgb(37, 99, 235);
            public static readonly Color SidebarInactive = Color.FromArgb(51, 65, 85);
            public static readonly Color SidebarText = Color.FromArgb(226, 232, 240);
            
            // Icon colors
            public static readonly Color IconActive = Color.White;
            public static readonly Color IconInactive = Color.FromArgb(226, 232, 240);
            public static readonly Color IconDisabled = Color.FromArgb(156, 163, 175);
        }
        
        public static class Typography
        {
            public static readonly Font HeaderLarge = new Font("Segoe UI", 28, FontStyle.Bold);
            public static readonly Font HeaderMedium = new Font("Segoe UI", 20, FontStyle.Bold);
            public static readonly Font HeaderSmall = new Font("Segoe UI", 16, FontStyle.Bold);
            public static readonly Font BodyLarge = new Font("Segoe UI", 14, FontStyle.Regular);
            public static readonly Font BodyMedium = new Font("Segoe UI", 12, FontStyle.Regular);
            public static readonly Font BodySmall = new Font("Segoe UI", 10, FontStyle.Regular);
            public static readonly Font ButtonText = new Font("Segoe UI", 11, FontStyle.Bold);
            public static readonly Font Caption = new Font("Segoe UI", 9, FontStyle.Regular);
        }
        
        /// <summary>
        /// Gets an icon from the Resources/Icons folder
        /// </summary>
        /// <param name="iconName">Name of the icon file (with extension)</param>
        /// <returns>Icon object or null if not found</returns>
        public static Icon? GetIcon(string iconName)
        {
            try
            {
                string iconPath = Path.Combine(Application.StartupPath, BaseResourcePath, "Icons", iconName);
                if (File.Exists(iconPath))
                {
                    return new Icon(iconPath);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading icon {iconName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets a FontAwesome icon with enhanced search capabilities
        /// </summary>
        /// <param name="iconName">Name of the FontAwesome icon file (with extension)</param>
        /// <param name="subfolder">Subfolder within FontAwesome directory (e.g., "regular", "solid", "brands")</param>
        /// <returns>Image object or null if not found</returns>
        public static Image? GetFontAwesomeIcon(string iconName, string subfolder = "solid")
        {
            try
            {
                string[] possiblePaths = {
                    Path.Combine(FontAwesomePath, "svgs", subfolder, iconName),
                    Path.Combine(FontAwesomePath, "svgs", "solid", iconName),
                    Path.Combine(FontAwesomePath, "svgs", "regular", iconName),
                    Path.Combine(FontAwesomePath, "svgs", "brands", iconName),
                    Path.Combine(FontAwesomePath, "webfonts", iconName),
                    Path.Combine(FontAwesomePath, "png", subfolder, iconName),
                    Path.Combine(FontAwesomePath, "icons", subfolder, iconName),
                    Path.Combine(FontAwesomePath, subfolder, iconName)
                };

                foreach (string iconPath in possiblePaths)
                {
                    if (File.Exists(iconPath))
                    {
                        if (Path.GetExtension(iconPath).ToLower() == ".svg")
                        {
                            return LoadSvgAsImage(iconPath);
                        }
                        else
                        {
                            return Image.FromFile(iconPath);
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading FontAwesome icon {iconName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Creates a colored icon from FontAwesome with advanced styling
        /// </summary>
        /// <param name="iconName">Name of the FontAwesome icon</param>
        /// <param name="color">Color to apply to the icon</param>
        /// <param name="size">Size of the icon in pixels</param>
        /// <param name="subfolder">FontAwesome subfolder</param>
        /// <returns>Colored bitmap image</returns>
        public static Bitmap? GetColoredFontAwesomeIcon(string iconName, Color color, Size size, string subfolder = "solid")
        {
            try
            {
                var svgPath = FindFontAwesomeIcon(iconName, subfolder);
                if (svgPath == null) return CreateFallbackIcon(size, color);

                if (Path.GetExtension(svgPath).ToLower() == ".svg")
                {
                    var svgDoc = SvgDocument.Open(svgPath);
                    svgDoc.Fill = new SvgColourServer(Color.FromArgb(color.A, color.R, color.G, color.B));
                    
                    var bitmap = svgDoc.Draw(size.Width, size.Height);
                    return EnhanceBitmap(bitmap, size);
                }
                else
                {
                    var originalImage = Image.FromFile(svgPath);
                    return ApplyColorToImage(originalImage, color, size);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating colored FontAwesome icon {iconName}: {ex.Message}");
                return CreateFallbackIcon(size, color);
            }
        }

        /// <summary>
        /// Enhances bitmap with anti-aliasing and smoothing
        /// </summary>
        private static Bitmap EnhanceBitmap(Bitmap original, Size targetSize)
        {
            if (original == null) return null;
            
            var enhanced = new Bitmap(targetSize.Width, targetSize.Height);
            using (Graphics g = Graphics.FromImage(enhanced))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                
                g.DrawImage(original, new Rectangle(0, 0, targetSize.Width, targetSize.Height));
            }
            
            return enhanced;
        }

        /// <summary>
        /// Comprehensive dashboard icons with fallback support
        /// </summary>
        public static class DashboardIcons
        {
            private static readonly Size DefaultIconSize = new Size(24, 24);
            private static readonly Size SmallIconSize = new Size(16, 16);
            private static readonly Size LargeIconSize = new Size(32, 32);

            // Navigation icons
            public static Bitmap? GetDashboardIcon(bool isActive = false) =>
                GetColoredFontAwesomeIcon("house.svg", isActive ? Theme.IconActive : Theme.IconInactive, DefaultIconSize) ??
                GetColoredFontAwesomeIcon("home.svg", isActive ? Theme.IconActive : Theme.IconInactive, DefaultIconSize) ??
                CreateFallbackIcon(DefaultIconSize, isActive ? Theme.IconActive : Theme.IconInactive, "D");

            public static Bitmap? GetInventoryIcon(bool isActive = false) =>
                GetColoredFontAwesomeIcon("boxes-stacked.svg", isActive ? Theme.IconActive : Theme.IconInactive, DefaultIconSize) ??
                GetColoredFontAwesomeIcon("boxes.svg", isActive ? Theme.IconActive : Theme.IconInactive, DefaultIconSize) ??
                CreateFallbackIcon(DefaultIconSize, isActive ? Theme.IconActive : Theme.IconInactive, "I");

            public static Bitmap? GetReportsIcon(bool isActive = false) =>
                GetColoredFontAwesomeIcon("chart-column.svg", isActive ? Theme.IconActive : Theme.IconInactive, DefaultIconSize) ??
                GetColoredFontAwesomeIcon("chart-bar.svg", isActive ? Theme.IconActive : Theme.IconInactive, DefaultIconSize) ??
                CreateFallbackIcon(DefaultIconSize, isActive ? Theme.IconActive : Theme.IconInactive, "R");

            public static Bitmap? GetSettingsIcon(bool isActive = false) =>
                GetColoredFontAwesomeIcon("gear.svg", isActive ? Theme.IconActive : Theme.IconInactive, DefaultIconSize) ??
                GetColoredFontAwesomeIcon("cog.svg", isActive ? Theme.IconActive : Theme.IconInactive, DefaultIconSize) ??
                CreateFallbackIcon(DefaultIconSize, isActive ? Theme.IconActive : Theme.IconInactive, "S");

            // CRUD operation icons
            public static Bitmap? GetAddIcon() =>
                GetColoredFontAwesomeIcon("plus.svg", Theme.Success, DefaultIconSize) ??
                CreateFallbackIcon(DefaultIconSize, Theme.Success, "+");

            public static Bitmap? GetEditIcon() =>
                GetColoredFontAwesomeIcon("pen-to-square.svg", Theme.Info, DefaultIconSize) ??
                GetColoredFontAwesomeIcon("edit.svg", Theme.Info, DefaultIconSize) ??
                CreateFallbackIcon(DefaultIconSize, Theme.Info, "E");

            public static Bitmap? GetDeleteIcon() =>
                GetColoredFontAwesomeIcon("trash.svg", Theme.Danger, DefaultIconSize) ??
                CreateFallbackIcon(DefaultIconSize, Theme.Danger, "X");

            public static Bitmap? GetViewIcon() =>
                GetColoredFontAwesomeIcon("eye.svg", Theme.TextSecondary, DefaultIconSize) ??
                CreateFallbackIcon(DefaultIconSize, Theme.TextSecondary, "V");

            public static Bitmap? GetSearchIcon() =>
                GetColoredFontAwesomeIcon("magnifying-glass.svg", Theme.TextSecondary, DefaultIconSize) ??
                GetColoredFontAwesomeIcon("search.svg", Theme.TextSecondary, DefaultIconSize) ??
                CreateFallbackIcon(DefaultIconSize, Theme.TextSecondary, "?");

            public static Bitmap? GetSaveIcon() =>
                GetColoredFontAwesomeIcon("floppy-disk.svg", Theme.Success, DefaultIconSize) ??
                GetColoredFontAwesomeIcon("save.svg", Theme.Success, DefaultIconSize) ??
                CreateFallbackIcon(DefaultIconSize, Theme.Success, "S");

            public static Bitmap? GetCancelIcon() =>
                GetColoredFontAwesomeIcon("xmark.svg", Theme.Danger, DefaultIconSize) ??
                GetColoredFontAwesomeIcon("times.svg", Theme.Danger, DefaultIconSize) ??
                CreateFallbackIcon(DefaultIconSize, Theme.Danger, "X");

            // User and system icons
            public static Bitmap? GetUserIcon() =>
                GetColoredFontAwesomeIcon("user.svg", Color.FromArgb(219, 234, 254), SmallIconSize) ??
                CreateFallbackIcon(SmallIconSize, Color.FromArgb(219, 234, 254), "U");

            public static Bitmap? GetLogoutIcon() =>
                GetColoredFontAwesomeIcon("right-from-bracket.svg", Color.White, SmallIconSize) ??
                GetColoredFontAwesomeIcon("sign-out-alt.svg", Color.White, SmallIconSize) ??
                CreateFallbackIcon(SmallIconSize, Color.White, ">");

            public static Bitmap? GetAppIcon() =>
                GetColoredFontAwesomeIcon("warehouse.svg", Color.White, LargeIconSize) ??
                GetColoredFontAwesomeIcon("building.svg", Color.White, LargeIconSize) ??
                CreateFallbackIcon(LargeIconSize, Color.White, "W");

            // Filter and sorting icons
            public static Bitmap? GetFilterIcon() =>
                GetColoredFontAwesomeIcon("filter.svg", Theme.TextSecondary, DefaultIconSize) ??
                CreateFallbackIcon(DefaultIconSize, Theme.TextSecondary, "F");

            public static Bitmap? GetSortIcon() =>
                GetColoredFontAwesomeIcon("sort.svg", Theme.TextSecondary, DefaultIconSize) ??
                CreateFallbackIcon(DefaultIconSize, Theme.TextSecondary, "?");

            public static Bitmap? GetRefreshIcon() =>
                GetColoredFontAwesomeIcon("rotate-right.svg", Theme.TextSecondary, DefaultIconSize) ??
                GetColoredFontAwesomeIcon("refresh.svg", Theme.TextSecondary, DefaultIconSize) ??
                CreateFallbackIcon(DefaultIconSize, Theme.TextSecondary, "?");
        }

        /// <summary>
        /// Creates styled fallback icons with text
        /// </summary>
        private static Bitmap CreateFallbackIcon(Size size, Color color, string text = "")
        {
            Bitmap fallback = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(fallback))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                
                // Draw background circle
                using (Brush brush = new SolidBrush(Color.FromArgb(50, color)))
                {
                    g.FillEllipse(brush, 1, 1, size.Width - 2, size.Height - 2);
                }
                
                // Draw border
                using (Pen pen = new Pen(color, 1))
                {
                    g.DrawEllipse(pen, 1, 1, size.Width - 3, size.Height - 3);
                }
                
                // Draw text if provided
                if (!string.IsNullOrEmpty(text))
                {
                    using (Font font = new Font("Segoe UI", size.Width / 3, FontStyle.Bold))
                    using (Brush textBrush = new SolidBrush(color))
                    {
                        var textSize = g.MeasureString(text, font);
                        var x = (size.Width - textSize.Width) / 2;
                        var y = (size.Height - textSize.Height) / 2;
                        g.DrawString(text, font, textBrush, x, y);
                    }
                }
            }
            return fallback;
        }

        /// <summary>
        /// Finds a FontAwesome icon file in various possible locations
        /// </summary>
        private static string? FindFontAwesomeIcon(string iconName, string subfolder)
        {
            string[] possiblePaths = {
                Path.Combine(FontAwesomePath, "svgs", subfolder, iconName),
                Path.Combine(FontAwesomePath, "svgs", "solid", iconName),
                Path.Combine(FontAwesomePath, "svgs", "regular", iconName),
                Path.Combine(FontAwesomePath, "icons", subfolder, iconName),
                Path.Combine(FontAwesomePath, subfolder, iconName)
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return null;
        }

        /// <summary>
        /// Applies color transformation to an image
        /// </summary>
        private static Bitmap ApplyColorToImage(Image originalImage, Color color, Size size)
        {
            Bitmap coloredBitmap = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(coloredBitmap))
            {
                g.Clear(Color.Transparent);
                using (Brush brush = new SolidBrush(color))
                {
                    g.FillRectangle(brush, new Rectangle(0, 0, size.Width, size.Height));
                }
                g.DrawImage(originalImage, new Rectangle(0, 0, size.Width, size.Height));
            }
            return coloredBitmap;
        }

        /// <summary>
        /// Loads SVG file as image using Svg.NET library
        /// </summary>
        private static Image? LoadSvgAsImage(string svgPath)
        {
            try
            {
                var svgDoc = SvgDocument.Open(svgPath);
                return svgDoc.Draw();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading SVG {svgPath}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets an image from the Resources/Icons folder
        /// </summary>
        /// <param name="imageName">Name of the image file (with extension)</param>
        /// <returns>Image object or null if not found</returns>
        public static Image? GetIconAsImage(string imageName)
        {
            try
            {
                string imagePath = Path.Combine(Application.StartupPath, BaseResourcePath, "Icons", imageName);
                if (File.Exists(imagePath))
                {
                    return Image.FromFile(imagePath);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image {imageName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets an image from the Resources/Images folder
        /// </summary>
        /// <param name="imageName">Name of the image file (with extension)</param>
        /// <returns>Image object or null if not found</returns>
        public static Image? GetImage(string imageName)
        {
            try
            {
                string imagePath = Path.Combine(Application.StartupPath, BaseResourcePath, "Images", imageName);
                if (File.Exists(imagePath))
                {
                    return Image.FromFile(imagePath);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading image {imageName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets a button icon from the Resources/Icons/buttons folder
        /// </summary>
        /// <param name="buttonIconName">Name of the button icon file (with extension)</param>
        /// <returns>Image object or null if not found</returns>
        public static Image? GetButtonIcon(string buttonIconName)
        {
            try
            {
                string iconPath = Path.Combine(Application.StartupPath, BaseResourcePath, "Icons", "buttons", buttonIconName);
                if (File.Exists(iconPath))
                {
                    return Image.FromFile(iconPath);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading button icon {buttonIconName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the full path to a resource file
        /// </summary>
        /// <param name="resourceType">Type of resource (Icons, Images, etc.)</param>
        /// <param name="fileName">Name of the file</param>
        /// <returns>Full path to the resource file</returns>
        public static string GetResourcePath(string resourceType, string fileName)
        {
            return Path.Combine(Application.StartupPath, BaseResourcePath, resourceType, fileName);
        }
    }
}