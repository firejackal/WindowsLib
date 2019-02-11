/*
' GDI+ Graphics Helper
' Version:     Build 5
' Description: Provides a easy framework for using the GDI+ graphics library, this code file
'              will provide a back buffer (to prevent flickering) and an images collection.
 * 
 * History:
 *  Build 6: Saturday, October 25th, 2014
 *      - (12:03) Update: Porting over to C#.
 *      
'   Build 5 - Thursday, April 30th, 2009
'     * (18:16) Update: Added support to detect if the client object is a form, and if it
'       is a form and has a menu, that menu's size and location will be accounted for when
'       automaticly determining the display area and where to draw the display to on
'       that client object. (Don't know if it works with Compact .NET)
'
'   Build 4 - Thursday, December 18th, 2008
'     * (17:54) Update: Compact .NET didn't support using the AllDirectories flag when 
'       finding all the image files in a folder, so wrote a new function which should do
'       Me.
'     * (17:54) Update: Updated the function 'SaveScreenshot()' to support the image format
'       since Compact .NET required Me.
'     * (17:59) Update: Updated the function 'FlipToWindow()' to support the only ways of
'       drawing images under Compact .NET.
'
'   Build 3 - Sunday, December 14th, 2008
'     * (17:23) Bugfix: Found a bug in the function 'AddFromPath()' for when computing the
'       automatic parent ID, if the ID began with a slash '\', it wasn't being correctly
'       removed.
'
'   Build 2 - Saturday, December 13th, 2008
'     * (17:41) New: Added support to save a screen shot of the buffer.
'     * (17:41) New: Added support for a custom buffer size.
'
'   Build 1 - Wednesday, July 30th, 2008
'     * (20:48) Created.
*/

using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace WindowsLib
{
    public class GDIPlusHelper
    {
        private Control  mContainer;
        private Form     mForm;
        private Bitmap   mOffscreenBitmap;
        private System.Drawing.Graphics mOffscreenGraphics;
        private GDIPlusImages     mImages;
        private InterpolationMode mInterpolationMode = InterpolationMode.Default;

        /// <summary>Create a new instance of the helper class and create the off-screen image.</summary>
        public GDIPlusHelper(Control container, int bufferWidth, int bufferHeight)
        {
            this.mContainer = container;
            this.mForm = (Form)this.mContainer;

            this.ResizeDisplay(bufferWidth, bufferHeight);

            this.mImages = new GDIPlusImages();
        } //Constructor

        /// <summary>Returns/Sets the interpolation mode used by the canvas.</summary>
        public InterpolationMode InterpolationMode
        {
            get { return this.mInterpolationMode; }
            set { this.mInterpolationMode = value; }
        } //InterpolationMode property

        /// <summary>Returns the canvas width.</summary>
        public int DisplayWidth { get { return this.mOffscreenBitmap.Width; } }

        /// <summary>Returns the canvas height.</summary>
        public int DisplayHeight { get { return this.mOffscreenBitmap.Height; } }

        /// <summary>Return the canvas.</summary>
        public System.Drawing.Graphics Display { get { return this.mOffscreenGraphics; } }

        /// <summary>Returns an instance to the images collection.</summary>
        public GDIPlusImages Images { get { return this.mImages; } }

        /// <summary>Resizes the canvas to our desired size.</summary>
        public bool ResizeDisplay(int newWidth, int newHeight)
        {
            if(this.mContainer == null) return false;

            Rectangle defaultSize = this.GetContainerRectangle();
            int imageWidth  = (newWidth  > 0 ? newWidth  : defaultSize.Width);
            int imageHeight = (newHeight > 0 ? newHeight : defaultSize.Height);

            if(this.mOffscreenGraphics != null) { this.mOffscreenGraphics.Dispose(); this.mOffscreenGraphics = null; }
            if(this.mOffscreenBitmap   != null) { this.mOffscreenBitmap.Dispose();   this.mOffscreenBitmap   = null; }

            this.mOffscreenBitmap = new Bitmap(imageWidth, imageHeight);

            this.mOffscreenGraphics = System.Drawing.Graphics.FromImage(this.mOffscreenBitmap);
            this.mOffscreenGraphics.SetClip(new Rectangle(0, 0, this.DisplayWidth, this.DisplayHeight));
            return true;
        } //ResizeDisplay function

        /// <summary>Returns the bitmap used by the canvas.</summary>
        public Bitmap DisplayBitmap { get { return this.mOffscreenBitmap; } }

        /// <summary>Returns a rectangle containing the location of the parent window/control.</summary>
        /// <returns></returns>
        private Rectangle GetContainerRectangle()
        {
            if(this.mContainer == null) return Rectangle.Empty;

            if(this.mForm == null || this.mForm.MainMenuStrip == null)
                return this.mContainer.ClientRectangle;
            else
                return new Rectangle(0, (this.mForm.MainMenuStrip.ClientRectangle.Top + this.mForm.MainMenuStrip.ClientRectangle.Height), this.mContainer.ClientRectangle.Width, this.mContainer.ClientRectangle.Height - (this.mForm.MainMenuStrip.ClientRectangle.Top + this.mForm.MainMenuStrip.ClientRectangle.Height));
        } //GetContainerRectangle function

        /// <summary>Display the canvas on the parent window/control.</summary>
        public void FlipToWindow()
        {
            if(this.mContainer == null || !this.mContainer.Visible) return;
            this.FlipToWindow(this.GetContainerRectangle());
        } //FlipToWindow function

        /// <summary>Display the canvas on the parent window/control.</summary>
        public void FlipToWindow(Rectangle clientArea)
        {
            if(this.mContainer == null || !this.mContainer.Visible) return;

            // Create on-screen graphics, this allows it to stretch on any sized display.
            System.Drawing.Graphics g = this.mContainer.CreateGraphics();
            if(this.mInterpolationMode != g.InterpolationMode) g.InterpolationMode = this.mInterpolationMode;
            // Copy off-screen image onto on-screen
            // the following line wasn't compatible in compact .net
            //g.DrawImage(this.mOffscreenBitmap, this.mContainer.ClientRectangle, 0, 0, this.mOffscreenBitmap.Width, this.mOffscreenBitmap.Height, GraphicsUnit.Pixel);
            g.DrawImage(this.mOffscreenBitmap, clientArea, new Rectangle(0, 0, this.mOffscreenBitmap.Width, this.mOffscreenBitmap.Height), GraphicsUnit.Pixel);
            // Destroy on-screen graphics
            g.Dispose();
        } //FlipToWindow function

        /// <summary>Saves a preview of the canvas to an image file.</summary>
        public void SaveScreenshot(string fileName)
        {
            ImageFormat format = ImageFormat.Bmp;
            switch(System.IO.Path.GetExtension(fileName).ToLower()) {
                case(".bmp"):  format = ImageFormat.Bmp; break;
                case(".gif"):  format = ImageFormat.Gif; break;
                case(".jpg"):
                case(".jpeg"): format = ImageFormat.Jpeg; break;
                case(".png"):  format = ImageFormat.Png; break;
            }
            this.mOffscreenBitmap.Save(fileName, format);
        } //SaveScreenshot function

        /// <summary>Blends two colors together.</summary>
        /// <param name="a">The first color.</param>
        /// <param name="b">The second color.</param>
        /// <param name="percent">The floating point percentage. 0.0f = 0%, 1.0f = 100%.</param>
        /// <returns>Returns the new color.</returns>
        public static Color BlendColors(Color a, Color b, float percent)
        {
            if(percent <= 0.0f) return a;
            if(percent >= 1.0f) return b;
            int Al = System.Convert.ToInt32((b.A * percent) + (a.A * (1.0f - percent)));
            int Re = System.Convert.ToInt32((b.R * percent) + (a.R * (1.0f - percent)));
            int Gr = System.Convert.ToInt32((b.G * percent) + (a.G * (1.0f - percent)));
            int Bl = System.Convert.ToInt32((b.B * percent) + (a.B * (1.0f - percent)));
            return Color.FromArgb(Al, Re, Gr, Bl);
        } //BlendColors function
    } //GDIPlusHelper

    public class GDIPlusImages : List<GDIPlusImage>
    {
        public GDIPlusImage Add(string id)
        {
            base.Add(new GDIPlusImage(id));
            return base[base.Count - 1];
        } //Add function

        public GDIPlusImage Add(string id, string fileName)
        {
            base.Add(new GDIPlusImage(id, fileName));
            return base[base.Count - 1];
        } //Add function

        public void AddFromPath(string pathName, string parentId, bool autoParentId = false)
        {
            string[] foundFiles = System.IO.Directory.GetFiles(pathName, "*.*", System.IO.SearchOption.AllDirectories);
            if(foundFiles == null || foundFiles.Length == 0) return;

            string imageID = "";
            foreach(string fileName in foundFiles) {
                switch(System.IO.Path.GetExtension(fileName).ToLower()) {
                    case(".bmp"):
                    case(".png"):
                    case(".gif"):
                    case(".jpg"):
                    case(".jpeg"):
                        if(autoParentId) {
                            string tempID = System.IO.Path.GetDirectoryName(fileName).Replace(RemoveDirSep(pathName), "");
                            if(tempID.StartsWith("\\") || tempID.StartsWith("/")) tempID = tempID.Substring(1);
                            imageID = MakeImageID(fileName, tempID);
                        } else {
                            imageID = MakeImageID(fileName, parentId);
                        }

                        this.Add(imageID, fileName);
                        break;
                }
            } //for fileName
        } //AddFromPath

        public int FindIndex(string id)
        {
            if(base.Count > 0) {
                for(int index = 0; index < base.Count; index++) {
                    if(string.Equals(base[index].Name, id, System.StringComparison.CurrentCultureIgnoreCase)) return index;
                } //for index
            }

            return -1;
        } //FindIndex function

        public int FindIndexByFile(string fileName)
        {
            if(base.Count > 0) {
                for(int index = 0; index < base.Count; index++) {
                    if(string.Equals(System.IO.Path.GetFileName(base[index].FileName), fileName, System.StringComparison.CurrentCultureIgnoreCase)) return index;
                } //for index
            }

            return -1;
        } //FindIndexByFile function

        public GDIPlusImage this[string id]
        {
            get {
                int index = this.FindIndex(id);
                if(index < 0) return null;
                return base[index];
            }
        } //Default Item Property

        public static string MakeImageID(string fileName, string parentID)
        {
            string filePart = System.IO.Path.GetFileNameWithoutExtension(fileName);

            if(string.IsNullOrEmpty(parentID))
                return filePart;
            else {
                if(filePart.StartsWith(parentID + ".", System.StringComparison.CurrentCultureIgnoreCase) ||
                    filePart.StartsWith(parentID + "_", System.StringComparison.CurrentCultureIgnoreCase) ||
                    filePart.StartsWith(parentID + " ", System.StringComparison.CurrentCultureIgnoreCase)) {
                    filePart = filePart.Substring((parentID + ".").Length);
                }

                return parentID + "." + filePart;
            }
        } //MakeImageID function

        private static string RemoveDirSep(string pathName)
        {
            if(pathName.EndsWith("\\") || pathName.EndsWith("/")) pathName = pathName.Substring(0, pathName.Length - 1);
            return pathName;
        } //RemoveDirSep function
    } //GDIPlusImages class

    public class GDIPlusImage
    {
        private string    mName        = "";
        private string    mFileName    = "";
        private Bitmap    mImage;
        private Rectangle mImageCoords = Rectangle.Empty;
        private Size      mImageSize   = Size.Empty;

        public GDIPlusImage(string name)
        {
            this.mName = name;
            this.mImageCoords = Rectangle.Empty;
        } //Constructor

        public GDIPlusImage(string name, string fileName)
        {
            this.mName = name;
            this.mImageCoords = Rectangle.Empty;
            this.LoadImage(fileName);
        } //Constructor

        public bool LoadImage(string fileName)
        {
            if(!System.IO.File.Exists(fileName)) return false;

            this.mFileName = fileName;
            this.Image = new Bitmap(fileName);
            return (this.Image != null);
        } //LoadImage function

        public string Name
        {
            get { return this.mName; }
            set { this.mName = value; }
        } //Name property

        public string FileName
        {
            get { return this.mFileName; }
            set { this.mFileName = value; }
        } //FileName property

        public Bitmap Image
        {
            get { return this.mImage; }
            set {
                this.mImage = value;
                this.mImageCoords = Rectangle.Empty;
                this.mImageSize = Size.Empty;
                if(this.Image != null) {
                    this.mImageCoords = new Rectangle(0, 0, this.mImage.Width, this.mImage.Height);
                    this.mImageSize = new Size(this.mImage.Width, this.mImage.Height);
                }
            }
        } //Image property

        public Rectangle Rectangle { get { return this.mImageCoords; } }

        public Size Size { get { return this.mImageSize; } }

        public int Width { get { return this.mImageCoords.Width; } }

        public int Height { get { return this.mImageCoords.Height; } }
    } //GDIPlusImage class
} //StrikeSoft.Libraries.Graphics namespace
