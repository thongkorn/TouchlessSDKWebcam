#Region "ABOUT"
' / -----------------------------------------------------------------------------
' / Developer : Mr.Surapon Yodsanga (Thongkorn Tubtimkrob)
' / eMail : thongkorn@hotmail.com
' / URL: http://www.g2gnet.com (Khon Kaen - Thailand)
' / Facebook: https://www.facebook.com/g2gnet (Thailand only)
' / Facebook: https://www.facebook.com/commonindy (Worldwide)
' / More Info: http://www.g2gsoft.com
' /
' / Purpose: Touchless SDK for Web camera and save images into disk.
' / Microsoft Visual Basic .NET (2010)
' /
' / This is open source code under @CopyLeft by Thongkorn/Common Tubtimkrob.
' / You can modify and/or distribute without to inform the developer.
' /
' / See more detail and download SDK ... http://touchless.codeplex.com/
' / --------------------------------------------------------------------------
#End Region

Imports System.Threading
Imports TouchlessLib
Imports System.IO

Public Class frmCamera
    Dim WebCamMgr As TouchlessLib.TouchlessMgr
    Dim strImagePath As String = MyPath(Application.StartupPath) & "Images\"

    Private Sub frmCamera_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            Timer1.Enabled = False
            WebCamMgr.CurrentCamera.Dispose()
            WebCamMgr.Cameras.Item(cmbCamera.SelectedIndex).Dispose()
            WebCamMgr.Dispose()
            Me.Dispose()
            GC.SuppressFinalize(Me)
            Application.Exit()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub frmCamera_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        WebCamMgr = New TouchlessLib.TouchlessMgr
        Label2.Text = ""
        For i As Integer = 0 To WebCamMgr.Cameras.Count - 1
            cmbCamera.Items.Add(WebCamMgr.Cameras(i).ToString)
        Next
        If cmbCamera.Items.Count >= 0 Then
            cmbCamera.SelectedIndex = 0
            Timer1.Enabled = True
            btnSave.Enabled = False
            '// Create a folder in VB if it doesn't exist.
            If (Not System.IO.Directory.Exists(strImagePath)) Then System.IO.Directory.CreateDirectory(strImagePath)
            '// Initialized and load images into DataGridView.
            Call InitDataGridView()
            Call LoadImage2DatagridView()
        Else
            MessageBox.Show("No Web Camera, This application needs a webcam.", "Report Status", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()
        End If
    End Sub

    ' / --------------------------------------------------------------------------
    Private Sub LoadImage2DatagridView()
        Dim directory As New IO.DirectoryInfo(strImagePath)
        If directory.Exists Then
            Dim jpgFiles() As IO.FileInfo = directory.GetFiles("*.jpg")
            Dim row As String()
            Dim iArr() As String
            For Each jpgFile As IO.FileInfo In jpgFiles
                iArr = Split(jpgFile.FullName, "\")
                row = New String() {iArr(UBound(iArr))}
                dgvData.Rows.Add(row)
                Using FS As IO.FileStream = File.Open(jpgFile.FullName, FileMode.Open)
                    Dim bitmap As Bitmap = New Bitmap(FS)
                    Dim currentPicture As Image = CType(bitmap, Image)
                    Me.dgvData.Rows(Me.dgvData.Rows.Count - 1).Cells(1).Value = currentPicture
                End Using
            Next
        End If
        Label2.Text = "Total Images : " & Me.dgvData.Rows.Count
    End Sub

    ' / --------------------------------------------------------------------------
    Private Sub btnSave_Click(sender As System.Object, e As System.EventArgs) Handles btnSave.Click

        If picPreview Is Nothing Then Exit Sub

        '/ Create file name
        Dim sTempFileName As String = strImagePath & Format(Now, "ddMMyy-HHmmss") & ".jpg"
        Dim b As Bitmap = picPreview.Image
        b.Save(sTempFileName, System.Drawing.Imaging.ImageFormat.Jpeg)
        '//
        Dim iArr() As String
        iArr = Split(sTempFileName, "\")
        Dim row As String()
        '// Show File name.
        row = New String() {iArr(UBound(iArr))}
        dgvData.Rows.Add(row)
        '// Load Image into DataGridView.
        Using FS As IO.FileStream = File.Open(sTempFileName, FileMode.Open)
            Dim bitmap As Bitmap = New Bitmap(FS)
            Dim currentPicture As Image = CType(bitmap, Image)
            Me.dgvData.Rows(Me.dgvData.Rows.Count - 1).Cells(1).Value = currentPicture
        End Using
        Label2.Text = "Total Images : " & Me.dgvData.Rows.Count
        Me.dgvData.Focus()
        SendKeys.Send("^{HOME}")
        SendKeys.Send("^{DOWN}")
        '// Delay Time. Prevent to save duplicate file name.
        btnSave.Enabled = False
        Threading.Thread.Sleep(1000) ' ms
        btnSave.Enabled = True
    End Sub

    ' / --------------------------------------------------------------------------
    Private Sub dgvData_DoubleClick(sender As Object, e As System.EventArgs) Handles dgvData.DoubleClick
        If Me.dgvData.Rows.Count = 0 Then Return
        frmShowImage.ShowDialog()
    End Sub

    ' / --------------------------------------------------------------------------
    Private Sub InitDataGridView()
        Me.dgvData.Columns.Clear()
        '// Initialize DataGridView Control
        With dgvData
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            '.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            '.AutoResizeColumns()
            .AllowUserToResizeColumns = True
            .AllowUserToResizeRows = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        End With
        dgvData.RowTemplate.Height = 120
        '// Declare columns type.
        '// Add 1th column (Index = 0), Show image file name.
        Dim FName As New DataGridViewTextBoxColumn()
        dgvData.Columns.Add(FName)
        With FName
            .HeaderText = "File Name"
            .ReadOnly = True
            .Visible = True
            .Width = Me.dgvData.Width \ 2 + 65
        End With
        '// Add 2th column (Index = 1), It's Image.
        Dim imgCol As New DataGridViewImageColumn()
        With imgCol
            .HeaderText = "Image"
            .Name = "img"
            .ImageLayout = DataGridViewImageCellLayout.Stretch
            .Width = 120
        End With
        dgvData.Columns.Add(imgCol)
        '//
        Me.dgvData.Focus()
    End Sub

    Private Sub cmbCamera_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbCamera.SelectedIndexChanged
        WebCamMgr.CurrentCamera = WebCamMgr.Cameras.ElementAt(cmbCamera.SelectedIndex)
        '//
        'WebCamMgr.CurrentCamera.CaptureHeight = 480
        'WebCamMgr.CurrentCamera.CaptureWidth = 640
    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        picFeed.Image = WebCamMgr.CurrentCamera.GetCurrentImage()
    End Sub

    Private Sub btnCapture_Click(sender As System.Object, e As System.EventArgs) Handles btnCapture.Click
        picPreview.Image = WebCamMgr.CurrentCamera.GetCurrentImage()
        btnSave.Enabled = True
    End Sub

    Private Sub frmCamera_Resize(sender As Object, e As System.EventArgs) Handles Me.Resize
        If Me.dgvData.Columns.Count > 0 Then
            With Me.dgvData.Columns(0)
                .Width = Me.dgvData.Width \ 2 + 65
            End With
        End If
    End Sub

    Private Sub btnDelete_Click(sender As System.Object, e As System.EventArgs) Handles btnDelete.Click
        If Me.dgvData.Rows.Count = 0 Then Return
        Try
            '// Delete files in folder.
            FileSystem.Kill(strImagePath & dgvData.CurrentRow.Cells(0).Value.ToString)
            '// Delete current row from dgvData
            dgvData.Rows.Remove(dgvData.CurrentRow)
            Label2.Text = "Total Images : " & Me.dgvData.Rows.Count
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

End Class
