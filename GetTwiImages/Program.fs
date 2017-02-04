open System
open System.Net
open System.Text.RegularExpressions
open System.IO

let fileList filepath =
    seq{
        use sr = new StreamReader(filepath:string)
        while not sr.EndOfStream do
            yield sr.ReadLine()
    }

let addList path =
    let lists = fileList path
    lists

let (|Pattern|_|) regex str =
    let result = Regex(regex).Match(str)
    if result.Success then
        Some(result.Groups.[1].Value)
    else
        None


let checkImage url =
    match url with
    | Pattern "(?!.*/)(\w*\.jpg|png)" url -> url
    | _ -> "None"

let dlImage url dir =
    printfn "Starting Download Sequence"
    let savefilename = checkImage url
    let wc = new WebClient()
    let saveurl = url + ":orig"
    let savedir = dir + "\\" + savefilename
    wc.DownloadFile(saveurl, savedir)

type Help (n) as self =
    do
        self.PutHelpText(n)
    member this.PutHelpText(n) =
        if n = 1 then
           printfn "

        Example:
            GetTwiImages.exe --savedir DirectoryName --list TextfilePath

            --savedir This is Directory to save files. Or not appoint Case is substitute MyPicutres Dir.
        
            --list This appoint Text file with download list written."


[<EntryPoint>]
let main argv = 
    if argv.Length <> 0 then
        let mutable savedir = ""
        let mutable listtxt = ""
        let mutable dir = ""
        let mutable num = 0
        for a in argv do
            if a = "--savedir" then
                num <- num + 1
                savedir <- argv.[num]
                num <- num - 1
            elif a = "--list" then
                num <- num + 1
                listtxt <- argv.[num]
                num <- num - 1
            num <- num + 1
        
        if savedir = "" then
           savedir <- System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)

        if listtxt <> "" then
            let urllist = addList listtxt

            for i in urllist do
                dlImage i savedir

        elif listtxt = "" then
            for b in argv do
                if checkImage(b) = "None" then
                    let h = new Help(1)
                    System.Environment.Exit(0)
                else
                    dlImage b savedir
                    
    else
        let h = new Help(1)
        System.Environment.Exit(0)

    0
