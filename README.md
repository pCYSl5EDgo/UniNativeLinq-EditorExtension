# UniNativeLinq-EditorExtension

A set of LINQ-like extension methods specialized for `NativeArray<T>` and `arrays of unmanaged types`.
UniNativeLinq is designed to replace LINQ methods in Unity's Burst Job.
GC.Alloc is prohibited in Burst Jobs and thus System.Linq methods are not able to be used.

UniNativeLinq now includes all System.Linq like apis except `ToLookp()`.

# How To Install

## Edit manifest.json

Find `Packages/manifest.json` in your project and edit it to look like this:

```yaml
{
  "dependencies": {
    "uninativelinq": "https://github.com/pCYSl5EDgo/UniNativeLinq-EditorExtension.git#2018.4",
    ...
  },
}
```

## Click Menu Item `Tools/UniNativeLinq/Import UniNativeLinq Essential Resources`

Start your Unity Editor and click menu item `Tools/UniNativeLinq/Import UniNativeLinq Essential Resources`.
Import all necessary assets to initialize.

## Include Assembly Reference to Your Assembly Definition Files

Select your working asmdef file to open the inspector.
Check `General/Override References` and `Assembly References` appears.
Add `UniNativeLinq.dll` to `Assembly References`.

# Conditions
## Supported Platforms

All platforms are supported!
  
## Supported Versions

 - 2018.4
 
# APIs

## Aggregate
## All
## Any
## Average
## Cast
## Contains
## Concat
## DefaultIfEmpty
## Distinct
## Except
## ForEach
## GroupBy
## GroupJoin
## Intersect
## Join
## MaxBy
## MinBy
## OrderBy
## Repeat
## Range
## Select
## SelectIndex
## Skip
## SkipWhile
## Sum
## Take
## TakeWhile
## TryGetFirst
## TryGetFirstIndexOf
## TryGetLast
## TryGetSingle
## Union
## Where
## WhereIndex
## WithIndex
## Zip
