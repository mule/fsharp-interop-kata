


module Win32 =
    open System
    open System.Text
    open System.Drawing 
    open System.Drawing.Imaging
    open System.Runtime.InteropServices
    type EnumWindowsProc = delegate of IntPtr * IntPtr -> bool

    
    [<Struct>]
    type RECT =
        val left:int
        val top:int
        val right:int
        val bottom:int

    
    [<DllImport("user32.dll")>]
    extern nativeint GetWindowText(IntPtr hwnd, StringBuilder sb, int nMaxCount)

    [<DllImport("user32.dll")>]
    extern bool GetWindowRect(IntPtr hwnd,  [<Out>] RECT& rect)

    [<DllImport("user32.dll")>]
    extern bool EnumWindows(EnumWindowsProc callback, IntPtr lParam)

    [<DllImport("user32.dll")>]
    extern bool PrintWindow(IntPtr hwnd, IntPtr hdc, UInt32 nFlags)

    
    [<DllImport("user32.dll")>]
    extern IntPtr GetForegroundWindow()


    let getWindowTitle windowHandle = 
        let buffer = new StringBuilder(100)
        let result = GetWindowText(windowHandle, buffer, 100 )
        buffer.ToString()

    let getWindowBounds windowHandle =
        let mutable rect : RECT = Unchecked.defaultof<_>
        let result = GetWindowRect(windowHandle, &rect)
        let bounds = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top)
        bounds

    let enumTopWindows() =
        let mutable windows = []
        let callback = new EnumWindowsProc(fun hwnd lparam -> 
            windows <- hwnd :: windows
            true  )
        let enumResult = EnumWindows(callback, IntPtr.Zero)
        windows

    let windowDataToString (title: string,  bounds: Rectangle) : string =
        let windowStr = sprintf " tittle: %s width: %d height: %d X: %d Y: %d" title bounds.Width bounds.Height bounds.X bounds.Y
        windowStr

    let capture( windowHandle: IntPtr, image: Bitmap) : Bitmap =         
        use g = Graphics.FromImage(image)
        let dc = g.GetHdc()
        let result = PrintWindow(windowHandle, dc, UInt32.MinValue) 
        g.ReleaseHdc(dc)
        image

    let imageSave(fileNameWithPath : string, image : Image) =
        image.Save(fileNameWithPath, ImageFormat.Png)

    let isValidWindowForCapturing( bounds : Rectangle) : bool = 
        bounds.Height > 0 && bounds.Width > 0


        
        
module Test = 
    printfn "%A" (Win32.enumTopWindows())


[<EntryPoint>]
let main argv =
    let foregroundWindowHandle = Win32.GetForegroundWindow()
    let windowTitle = Win32.getWindowTitle(foregroundWindowHandle)
    let windowBounds = Win32.getWindowBounds(foregroundWindowHandle)
    let filename = System.Guid.NewGuid().ToString() |> sprintf "C:\\Temp\\%s.png" 
    printf "\nCapturing %s to file %s" windowTitle filename
    use mutable image = new System.Drawing.Bitmap(windowBounds.Width, windowBounds.Height)
    let image = Win32.capture(foregroundWindowHandle, image)
    Win32.imageSave(filename, image)
    0 // return an integer exit code