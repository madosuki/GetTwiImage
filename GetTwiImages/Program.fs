open System
open System.Net
open System.Text.RegularExpressions
open System.IO

let fileList filepath =
    seq{
        use sr = new StreamReader(filepath:string)
        while not sr.EndOfStream do
            yield sr.ReadLine()
        sr.Close()
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
    | Pattern "(?!.*/)(\S*\.(jpg|png))" url -> url
    | _ -> "None"

let (|ArgsTest|_|) str =
    if checkImage(str) <> "None" then
        Some(str)
    else
        None

let dlImage url dir =
    let savefilename = checkImage url
    if savefilename <> "None" then
        let wc = new WebClient()
        let mutable savedir = ""
        let saveurl = url + ":orig"
        let tmpdir:string = dir
        if tmpdir.EndsWith("/") then
            savedir <- tmpdir + savefilename
        else
            savedir <- tmpdir + "/" + savefilename
        printfn ""
        printfn "Starting Download Sequence: %s" url
        try
            wc.DownloadFile(saveurl, savedir)
            printfn ""
            printfn "Saved %s" savedir
            printfn "Done."
        with
        | :? WebException as ex -> printfn "%s" (ex.Message)

    else
        printfn "This URL is not twimg."

type Help (n) as self =
    do
        self.PutHelpText(n)
    member this.PutHelpText(n) =
        if n = 1 then
           printfn "

Example:
       GetTwiImages.exe --savedir DirectoryName --list TextfilePath Or twimg Url, In that case don't use --list option

       --savedir This is Directory to save files. Or not appoint Case is substitute MyPicutres Dir.
        
       --list This appoint Text file with download list written."

 type FiledArgs (n) =
     do
     if n = None then
         let h = new Help(1)
         System.Environment.Exit(0)


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
                match b with
                | ArgsTest b -> dlImage b savedir
                | _ -> ()
                    
    else
        let h = new Help(1)
        System.Environment.Exit(0)

    0
