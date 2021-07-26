Module modFunction
    ' / Get my project path
    ' / Ex. 
    ' / AppPath = C:\My Project\bin\debug
    ' / Replace "\bin\debug" with "\"
    ' / Return : C:\My Project\
    Public Function MyPath(AppPath As String) As String
        '/ MessageBox.Show(AppPath);
        AppPath = AppPath.ToLower()
        MyPath = AppPath.Replace("\bin\debug", "\").Replace("\bin\release", "\")
        '// Check the backslash symbol (ASCII Code = 92) on the far right. If not, add one at the end.
        If Microsoft.VisualBasic.Right(MyPath, 1) <> Chr(92) Then MyPath = MyPath & Chr(92)
    End Function

End Module
