

module Win32 =
    open System
    open System.Text
    open System.Drawing 
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


    let getWindowTitle windowHandle = 
        let buffer = new StringBuilder(100)
        let result = GetWindowText(windowHandle, buffer, 100 )
        buffer

    let getWindowBounds windowHandle =
        let mutable rect : RECT = Unchecked.defaultof<_>
        let result = GetWindowRect(windowHandle, &rect)
        rect

    let enumTopWindows() =
        let mutable windows = []
        let callback = new EnumWindowsProc(fun hwnd lparam -> 
            windows <- hwnd :: windows
            true  )
        let enumResult = EnumWindows(callback, IntPtr.Zero)
        windows

    let windowToString (title: string,  bounds: RECT) : string =
        let windowStr = sprintf " tittle: %s left: %d top: %d right %d bottom: %d" title bounds.left bounds.top bounds.right bounds.bottom
        windowStr
        
        
        
module Test = 
    printfn "%A" (Win32.enumTopWindows())


[<EntryPoint>]
let main argv =
    let windowDataList = Win32.enumTopWindows() |> List.map( fun windowHandle -> ( Win32.getWindowTitle(windowHandle).ToString(), Win32.getWindowBounds(windowHandle) ))
    windowDataList |> List.iter( fun windowData ->(Win32.windowToString( fst windowData, snd windowData) |> printf "%s\n" ))
    0 // return an integer exit code