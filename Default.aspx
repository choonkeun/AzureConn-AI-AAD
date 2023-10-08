<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AzureConn.Default" %>
<%@ Register tagPrefix="AzureConn" namespace="AzureConn" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 224px;
        }
        .auto-style2 {
            margin-bottom: 0px;
        }
        .auto-style3 {
            width: 224px;
            height: 20px;
        }
        .auto-style4 {
            height: 20px;
        }
        .btn {
            display: inline-block;
            margin-bottom: 0;
            font-weight: 400;
            text-align: center;
            white-space: nowrap;
            vertical-align: middle;
            touch-action: manipulation;
            cursor: pointer;
            background-image: none;
            border: 1px solid transparent;
            padding: 6px 12px;
            font-size: 14px;
            line-height: 1.42857143;
            border-radius: 4px;
            user-select: none;
        }
        .btn-primary {
            color: #fff;
            background-color: #1b6ec2;
            border-color: #1861ac;
        }
        .btn-success {
            color: #fff;
            background-color: #5cb85c;
            border-color: #4cae4c;
        }

        /*partners > contentData CSS */
        .styled-table {
          width: 850px;
          border-collapse: collapse;
          margin: 25px 0;
          font-size: 0.9em;
          font-family: sans-serif;
          min-width: 400px;
          box-shadow: 0 0 20px rgba(0, 0, 0, 0.15);
        }

        .styled-table thead tr {
          background-color: #009879;
          color: #ffffff;
          text-align: left;
        }

        .styled-table th, 
        .styled-table td {
          padding: 5px 15px;
        }

        .styled-table tbody td {
          /*border-bottom: 1px solid #dddddd;*/
          border: 1px solid #dddddd;
        }

        .styled-table tbody tr:nth-of-type(even) {
          background-color: #f3f3f3;
        }

        .styled-table tbody tr:last-of-type {
          border-bottom: 2px solid #009879;
        }
        #lblUploadResult {
          border-bottom: 2px solid #009879;
        }

        .styled-table tbody tr:hover {background-color: lightblue;}

    </style>
</head>
<body>
    <form id="form1" runat="server" method="post" EncType="multipart/form-data" action="Default.aspx">

        <div>
            <table style="width:100%;">
                <tr>
                    <td class="auto-style1">Full Address</td>
                    <td>
            <asp:TextBox ID="txtFullAddress" runat="server" Width="600px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style1">CNAme Address</td>
                    <td>
                        <asp:TextBox ID="txtCNameAddress" runat="server" Width="600px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style1">Database Name</td>
                    <td><asp:TextBox ID="txtDatabaseName" runat="server" Width="600px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style1">User Id</td>
                    <td><asp:TextBox ID="txtUserId" runat="server" Width="300px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style1">Password</td>
                    <td>
                        <asp:TextBox ID="txtPassword" runat="server" Width="300px"></asp:TextBox> (web, prod or any or type password)
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">
                        Query string</td>
                    <td class="auto-style4">
                        <asp:TextBox ID="txtQuery" runat="server" Width="600px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">
                        Indented</td>
                    <td class="auto-style4">
                        <asp:RadioButton ID="rdoTrue" GroupName="radIndent" runat="server" />
                        <label for="rdoTrue">True</label> 
                        <asp:RadioButton ID="rdoFalse" GroupName="radIndent" runat="server" Checked="true" />
                        <label for="rdoFalse">False</label> 
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">
                        <br />
                        Selected Server name</td>
                    <td class="auto-style4">
                        <br />
                        <asp:Label ID="Label1" runat="server" Width="600px" CssClass="auto-style2"></asp:Label>
                    </td>
                </tr>
            </table>
            &nbsp;<br />
            <br />
            <table style="width:100%;">
            <tr>
                <td  class="auto-style1">
                    <asp:Button ID="btnFullAddress" runat="server" Text="Full Address" OnClick="btnFullAddress_Click" />&nbsp;
                    <asp:Button ID="btnCNameAddress" runat="server" Text="CNAME" OnClick="btnCNameAddress_Click" />&nbsp;&nbsp;
                </td>
                <td>
                    <asp:Button ID="btnUpload" runat="server" Text="Upload" OnClick="btnUpload_Click" ></asp:Button>&nbsp;
                    <INPUT id="oFile" multiple type="file" runat="server" /> 
                    <asp:Button ID="btnViewImages" Text="View Images" runat="server" OnClick="btnViewImages_Click"></asp:Button>
                </td>
            </tr>
            </table>
            <br />
            <asp:Panel ID="pnResults" Visible="true" runat="server">
                Results :<br />
                <asp:TextBox ID="txtResults" runat="server" Height="301px" TextMode="MultiLine" Width="807px"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="pnImageFiles" Visible="False" runat="server">
                Results :<br />
                <asp:Label id="lblUploadResult" Runat="server"></asp:Label>
            </asp:Panel>

            <asp:Panel ID="pnImageTable" runat="server" CssClass="table"  Visible="False" >
                <asp:Label id="lblTableMessage" Runat="server"></asp:Label>
                <asp:Table ID="UploadTable" CssClass="styled-table" runat="server" >
                    <asp:TableHeaderRow TableSection="TableHeader">
                        <asp:TableHeaderCell HorizontalAlign="Center">FileName</asp:TableHeaderCell>
                        <asp:TableHeaderCell HorizontalAlign="Center" Width="33%">Date</asp:TableHeaderCell>
                        <asp:TableHeaderCell HorizontalAlign="Center" Width="33%">action</asp:TableHeaderCell>
                    </asp:TableHeaderRow>

                    <asp:TableFooterRow>
                      <asp:TableCell >
                          <asp:Label ID="lblImageFileCount" runat="server"></asp:Label>
                       </asp:TableCell>
                      <asp:TableCell Text=""  />
                      <asp:TableCell>
                        <asp:Button ID="btnDeleteImage" runat="server" OnClick="btnDeleteImage_Click" Text="Delete Image" />
                      </asp:TableCell>
                    </asp:TableFooterRow>

                </asp:Table>
                <br /><br />
            </asp:Panel>

        </div>

<div>
    <asp:Button ID="btnSignIn" CssClass="btn btn-success" runat="server" Text="SignIn" OnClick="btnSignIn_Click" />
    <asp:Button ID="btnSignOut" CssClass="btn btn-primary" runat="server" Text="SignOut" OnClick="btnSignOut_Click" />
</div>
       
    </form>
</body>
</html>
