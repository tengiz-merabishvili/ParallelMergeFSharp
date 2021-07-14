open System
open System.Threading.Tasks
open System.Threading
open Sort

[<EntryPoint>]
let main argv = 
    let recordTime func =
        GC.Collect()
        GC.Collect(GC.MaxGeneration)
        GC.WaitForFullGCComplete() |> ignore
        GC.WaitForPendingFinalizers()
        let sw = new System.Diagnostics.Stopwatch()
        sw.Start()
        func()
        sw.Elapsed

    // ფუნქცია ბეჭდავს ალგორითმის მუშაობის დროს და ელემენტების რაოდენობას
    let writeTime (sortCount:int) (timespan : TimeSpan) = 
        printfn "Sort took %f seconds : Element count = %i" timespan.TotalSeconds sortCount
    
    let parallelProcessCount = Environment.ProcessorCount
    let numberOfElements = 24

    //დავბეჭდოთ ალგორითმის მუშაობის დრო და ელემენტების რაოდებობა
    recordTime (fun () ->
        Sort.parallelMergeSort(parallelProcessCount, numberOfElements)) 
    |> writeTime numberOfElements

    0
