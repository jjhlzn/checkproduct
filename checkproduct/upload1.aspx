<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="upload1.aspx.cs" Inherits="checkproduct.upload1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
    <div>
        <input type="file" name="pic0" accept="image/*" />
        <input type="file" name="pic1" accept="image/*" />
        <input type="submit" />
    </div>
    </form>
</body>
</html>
