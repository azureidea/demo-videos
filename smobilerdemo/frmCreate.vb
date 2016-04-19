Imports Smobiler.Core
Imports System.Data.SqlClient
Imports System.Text
Imports System.Windows.Forms

Public Class frmCreate
    Inherits Smobiler.Core.MobileForm

    'toolbar�¼�����ʱ�߼�
    Private Sub frmCreate_ToolbarItemClick(sender As Object, e As ToolbarClickEventArgs) Handles Me.ToolbarItemClick
        Try
            'ѡ�񴥷����¼�����
            Select Case e.Name
                '�����¼�
                Case Save.Name
                    If txtNO.Text.Trim.Length <= 0 Then Throw New Exception("������ͻ���š�")
                    If txtName.Text.Trim.Length <= 0 Then Throw New Exception("������ͻ�������")
                    Dim co As New OleDbConnection
                    co.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\smobilerdemo.accdb;Persist Security Info=True"
                    co.Open()
                    Dim myCommand As New OleDbCommand("insert into tblCustomer(CUST_ID,CUST_NAME,INV_CONTENT)values(@CUST_ID,@CUST_NAME,@INV_CONTENT)", co)
                    myCommand.Parameters.Add(New OleDbParameter("@CUST_ID", OleDbType.BSTR, 10))
                    myCommand.Parameters("@CUST_ID").Value = txtNO.Text

                    myCommand.Parameters.Add(New OleDbParameter("@CUST_NAME", OleDbType.BSTR, 50))
                    myCommand.Parameters("@CUST_NAME").Value = txtName.Text

                    myCommand.Parameters.Add(New OleDbParameter("@INV_CONTENT", OleDbType.BSTR, 255))
                    myCommand.Parameters("@INV_CONTENT").Value = txtContact.Text
                    Dim retint As Integer = myCommand.ExecuteNonQuery()
                    myCommand.Connection.Close()
                    If retint > 0 Then MessageBox.Show("�����ɹ���")
                    co.Close()
                    '�����¼�
                Case Back.Name
                    ' �رյ�ǰ����
                    Me.Close()
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
End Class
