using System.Collections.Immutable;

namespace BlazorTextEditor.RazorLib.Analysis.JavaScript;

public static class JavaScriptKeywords
{
    public const string AwaitKeyword = "await";
    public const string BreakKeyword = "break";
    public const string CaseKeyword = "case";
    public const string CatchKeyword = "catch";
    public const string ClassKeyword = "class";
    public const string ConstKeyword = "const";
    public const string ContinueKeyword = "continue";
    public const string DebuggerKeyword = "debugger";
    public const string DefaultKeyword = "default";
    public const string DeleteKeyword = "delete";
    public const string DoKeyword = "do";
    public const string ElseKeyword = "else";
    public const string EnumKeyword = "enum";
    public const string ExportKeyword = "export";
    public const string ExtendsKeyword = "extends";
    public const string FalseKeyword = "false";
    public const string FinallyKeyword = "finally";
    public const string ForKeyword = "for";
    public const string FunctionKeyword = "function";
    public const string IfKeyword = "if";
    public const string ImplementsKeyword = "implements";
    public const string ImportKeyword = "import";
    public const string InKeyword = "in";
    public const string InstanceofKeyword = "instanceof";
    public const string InterfaceKeyword = "interface";
    public const string LetKeyword = "let";
    public const string NewKeyword = "new";
    public const string NullKeyword = "null";
    public const string PackageKeyword = "package";
    public const string PrivateKeyword = "private";
    public const string ProtectedKeyword = "protected";
    public const string PublicKeyword = "public";
    public const string ReturnKeyword = "return";
    public const string SuperKeyword = "super";
    public const string SwitchKeyword = "switch";
    public const string StaticKeyword = "static";
    public const string ThisKeyword = "this";
    public const string ThrowKeyword = "throw";
    public const string TryKeyword = "try";
    public const string TrueKeyword = "True";
    public const string TypeofKeyword = "typeof";
    public const string VarKeyword = "var";
    public const string VoidKeyword = "void";
    public const string WhileKeyword = "while";
    public const string WithKeyword = "with";
    public const string YieldKeyword = "yield";
    
    public static readonly ImmutableArray<string> All = new[]
    {
        AwaitKeyword,
        BreakKeyword,
        CaseKeyword,
        CatchKeyword,
        ClassKeyword,
        ConstKeyword,
        ContinueKeyword,
        DebuggerKeyword,
        DefaultKeyword,
        DeleteKeyword,
        DoKeyword,
        ElseKeyword,
        EnumKeyword,
        ExportKeyword,
        ExtendsKeyword,
        FalseKeyword,
        FinallyKeyword,
        ForKeyword,
        FunctionKeyword,
        IfKeyword,
        ImplementsKeyword,
        ImportKeyword,
        InKeyword,
        InstanceofKeyword,
        InterfaceKeyword,
        LetKeyword,
        NewKeyword,
        NullKeyword,
        PackageKeyword,
        PrivateKeyword,
        ProtectedKeyword,
        PublicKeyword,
        ReturnKeyword,
        SuperKeyword,
        SwitchKeyword,
        StaticKeyword,
        ThisKeyword,
        ThrowKeyword,
        TryKeyword,
        TrueKeyword,
        TypeofKeyword,
        VarKeyword,
        VoidKeyword,
        WhileKeyword,
        WithKeyword,
        YieldKeyword,
    }.ToImmutableArray();
}