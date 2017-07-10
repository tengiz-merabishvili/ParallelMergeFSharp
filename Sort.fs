module Sort

open System
open System.Threading.Tasks
open System.Threading

let parallelMergeSort(parallelProcessCount, numberOfElements) =
    let cores = parallelProcessCount
    let rand = new Random()
    let array = Array.create numberOfElements 0

    //შევქმნათ შემთხვევითი რიცვვების მასივი
    for i in 0..array.Length - 1 do
        array.[i] <- rand.Next(1, 300)
    
    //მასივს გავყოფთ ბირთვების რაოდენობაზე
    let chunks = Array.create cores [||]
    //ფუნქცია რომელიც ამატებს ელემენტს ყოველი ბირთვის შესაბამის მასივში და ასორტირებს
    let Add index number =
            let temporaryArray = Array.append [|number|] chunks.[index]
            chunks.[index] <- Array.sort temporaryArray
    
    //ორი მასივის შერწყმა
    let mergeTwoArrays (firstArray: int[]) (secondArray: int[]) : int[] = 
        let mutable firstIndex = 0
        let mutable secondIndex = 0
        let finalArray = Array.zeroCreate (firstArray.Length + secondArray.Length)
        
        for i in 0..finalArray.Length - 1 do
            
            if (firstIndex >= firstArray.Length) then
                finalArray.[i] <- secondArray.[secondIndex]
                secondIndex <- secondIndex + 1
            elif (secondIndex >= secondArray.Length) then
                finalArray.[i] <- firstArray.[firstIndex]
                firstIndex <- firstIndex + 1
            elif (firstArray.[firstIndex] < secondArray.[secondIndex]) then
                finalArray.[i] <- firstArray.[firstIndex]
                firstIndex <- firstIndex + 1
            else
                finalArray.[i] <- secondArray.[secondIndex]
                secondIndex <- secondIndex + 1

        finalArray
    
    //იტერაციული ფუნქცია რომელიც აკეტებს შერწყმას mergeTwoArrays გამოძახებით
    let rec mergeIteration(arr: int[][]) : int[] =
        if (arr.Length = 1) then
           arr.[0]
        else
            let length = arr.Length / 2
            let copyArray = Array.create length [||]
            for i in 0..length - 1 do
                copyArray.[i] <- mergeTwoArrays arr.[i * 2] arr.[i * 2 + 1]
            mergeIteration(copyArray)

    //პარალელური ნაკადების გაშვების საშუალებით ასინქრონულად ვალაგებთ მასივებს
    for i in 0..cores..array.Length - 1 do
        let tasks : Task<'T> [] = Array.zeroCreate cores
        for j in 0..cores - 1 do
            let coreIndex = j
            let elementIndex = i + j
            tasks.[coreIndex] <- Task.Factory.StartNew<'T>(fun () -> Add coreIndex array.[elementIndex])
        Task.WaitAll [| for task in tasks -> task :> Task |]

    let finalArray = mergeIteration chunks
    printfn "%A" finalArray


