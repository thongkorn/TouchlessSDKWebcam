Public Class frmShowImage

    Private Sub btnExit_Click(sender As System.Object, e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub frmShowImage_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        picData.Image = Image.FromFile(MyPath(Application.StartupPath) & "Images\" & frmCamera.dgvData.CurrentRow.Cells(0).Value.ToString)
    End Sub
End Class