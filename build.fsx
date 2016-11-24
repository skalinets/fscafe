#r "packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing.XUnit2

let buildDir = "./build"
let testsDir = "./tests"

Target "Clean" (fun _ -> CleanDirs [buildDir; testsDir])

Target "BuildApp" (fun _ -> 
        !! "src/**/*.fsproj"
            -- "src/**/*Tests.fsproj"
            |> MSBuildRelease buildDir "Build"
            |> Log "AppBuild-Output: "
)

Target "BuildTests" (fun _ -> 
        printfn "hello %A" "you"
        !! "src/**/*Tests.fsproj"
            |> MSBuildDebug testsDir "Build"
            |> Log "TestBuild-Output: "
)
let xunitRunnerPath = "./packages/xunit.runner.console/tools/xunit.console.exe"

Target "RunUnitTests" (fun _ ->
        !! (testsDir @@ "*Tests.dll")
        |> xUnit2 (fun p -> { p with ToolPath = xunitRunnerPath})

        printfn "hello"
)

"Clean"
    ==> "BuildApp"
    ==> "BuildTests"
    ==> "RunUnitTests"

RunTargetOrDefault "RunUnitTests"
