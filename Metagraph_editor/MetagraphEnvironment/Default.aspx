<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1.WebForm1" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>БНЗ</title>
    <link rel="stylesheet" type="text/css" href="Style/css/NEW.css" media="screen" />
    <link rel="stylesheet" href="Style/css/style.css" />
</head>
<body>
    <form id="form1" runat="server">
        <header>
              
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
        </header>


        <div id="Main">
            <div id="HeadPanel" runat="server">
                <div id = "FileUpload">
                    <asp:FileUpload ID="FileUploadOne" runat="server" CssClass="HeadPanelButton"/>     
                </div>
                <asp:Button ID="CreateNewBNZ" runat="server" OnClick="UploadFileCreateNew" Text="Створити нову БНЗ" CssClass="HeadPanelButton"/>
                <asp:Button ID="AddToTheExistingBNZ" runat="server" OnClick="UploadFileAddToOld" Text="Доповнити стару БНЗ" CssClass="HeadPanelButton"/>
                <asp:Button runat="server" ID="SaveBNZ" Text="Зберегти БНЗ" OnClick="SaveBNZ_Click" CssClass="HeadPanelButton"/>   
                <asp:Button ID="GetMetagraph" runat="server" OnClick ="Metagraph_Click" Text="Отримати метаграф" CssClass="HeadPanelButton"/>                  
            </div>
            <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="2" TabStripPlacement="TopRight" AutoPostBack="True" CssClass="Tabs">
                <cc1:TabPanel runat="server" HeaderText="TabPanel1" ID="TabPanel1">
                    <HeaderTemplate> <div class="TabHeader">РЕДАГУВАННЯ БНЗ </div></HeaderTemplate>
                    <ContentTemplate>
                        <hr />
                        <div id="Tree">
                            <asp:UpdatePanel ID="TreePan" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                         
                                    <a>Введіть лінгвістичну змінну:</a>
                                            <br />
                                    <asp:TextBox ID="NameLP" runat="server" CssClass="TextBox"></asp:TextBox>
                                          <br />
                                    <a>Скорочення для ЛЗ:</a> 
                                            <br />
                                    <asp:TextBox ID="ShortNameLP" runat="server" CssClass="TextBox"></asp:TextBox>
                                         <br />
                                    <a>Область визначення:</a>
                                        <br />
                                    <a>Min:</a>
                                    <asp:TextBox ID="MinValueLP" runat="server" CssClass="MinMaxTextBox"></asp:TextBox>
                                     
                                    <a>Max:</a>
                                    <asp:TextBox ID="MaxValueLP" runat="server" CssClass="MinMaxTextBox"></asp:TextBox>
                                    <asp:Button ID="PlusLPButton" CssClass="plusButton" runat="server" Text="+" OnClick="AddLPFromInterface_Click"/>
                                           <br />
                                    <a>Введіть терм:</a>
                                            <br />
                                    <asp:TextBox ID="NameTerm" runat="server" CssClass="TextBox"></asp:TextBox>
                                     
                                    <asp:Button ID="PlusTermButton" runat="server" CssClass="plusButton" OnClick="AddTermFromInterface_Click" Text="+"/>
                                        
                                    <a>Редагування:</a>
                                           
                                    <asp:TextBox ID="EditRuleTextBox" runat="server" OnTextChanged="EditRule_onTextChange" CssClass="TextBox"></asp:TextBox>
                                    <asp:ImageButton ID="EditLPTermImageButton" Width="9.8%" Height="9.8%" CssClass="Pencil" runat="server" ImageUrl="PEN_2.png"/>
                                    <asp:TreeView ID="TreeViewBNZ" runat="server" OnSelectedNodeChanged="TreeViewBNZ_SelectedNodeChanged" OnTreeNodeCheckChanged="TreeViewBNZ_TreeNodeCheckChanged">
                                        <NodeStyle CssClass="node" HorizontalPadding="3px" />
                                        <ParentNodeStyle CssClass="node" />
                                        <SelectedNodeStyle CssClass="node" />
                                    </asp:TreeView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="Main_text" >
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate><asp:DropDownList ID="Type_FP_List" runat="server" Visible="false">
                                    <asp:ListItem Value="1">Трикутна</asp:ListItem>
                                                 </asp:DropDownList>
                                     
                                    <div id="fp">
                                        <asp:Chart ID="Chart1" runat="server" visible="false" Width="1000px">
                                        <Series></Series>
                                        <ChartAreas><asp:ChartArea Name="ChartArea1"></asp:ChartArea></ChartAreas></asp:Chart>
                                        <asp:UpdatePanel ID ="xyz" runat="server" UpdateMode="Conditional" Visible="false">
                                            <ContentTemplate>
                                                 
                                                <a>Точка A:   (</a> <asp:TextBox ID="X" runat="server" AutoPostBack="true" OnTextChanged="X_TextChanged" CssClass="TextBox"></asp:TextBox> <a>; 0)</a>
                                                 <br />
                                                <a>Точка B:   (</a> <asp:TextBox ID="Y" runat="server" AutoPostBack="true" OnTextChanged="Y_TextChanged" CssClass="TextBox"></asp:TextBox> <a>; 1)</a>
                                                 <br />
                                                <a>Точка C:   (</a> <asp:TextBox ID="Z" runat="server" AutoPostBack="true" OnTextChanged="Z_TextChanged" CssClass="TextBox"></asp:TextBox> <a>; 0)</a>
                                                 
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <hr style="height: -15px" /><h3></h3>
                                    <div id="Edit_Rules" runat="server">
                                        <asp:Button ID="IfButton" runat="server" CssClass="BigButton" OnClick="IfButton_Click" Text="ЯКЩО"/>
                                        <asp:Button ID="ThenButton" runat="server" CssClass="BigButton" OnClick="ThenButton_Click" Text="ТО"/>

                                        <asp:ListBox ID="IfListToEditRule" runat="server" Visible ="false" BorderWidth ="0" Rows="7" Overflow="hidden"></asp:ListBox>
                                         
                                        <asp:Button ID="MinusIfToEditRule" CssClass="Button" runat="server" Visible="false" OnClick="MinusIfToEditRule_Click" Text="Прибрати зі списку" z-index="5"/>
                                            
                                        <asp:Label ID="ThenLabelToEditRule" runat="server" z-index="5"></asp:Label>
                                            <br />
                                        <asp:Button ID="SaveRule" CssClass="Button" runat="server" OnClick="SaveRule_Click" Text="Зберегти правило"/>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </ContentTemplate>
                </cc1:TabPanel>
                 <cc1:TabPanel runat="server" ID="MetagraphTabPanel">
                    <HeaderTemplate> <div class="TabHeader">МЕТАГРАФ</div> </HeaderTemplate>
                    <ContentTemplate>
                        <hr />
                        </ContentTemplate>
                     </cc1:TabPanel>
                <cc1:TabPanel runat="server" HeaderText="TabPanel1" ID="TabPanel2">
                    <HeaderTemplate><div class="TabHeader">НЕЧІТКЕ ВИВЕДЕННЯ</div></HeaderTemplate>
                    <ContentTemplate>
                        <hr />
                        <div id="text">
                            <asp:UpdatePanel ID="ConcludeUpdatePanel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <a class ="BigText">Виберіть із списку ЛП, яка вас цікавить:</a>
                                         
                                    <asp:DropDownList CssClass="DropDownList" ID="ConcludeLP" runat="server" AutoPostBack="true">
                                    </asp:DropDownList>
                                         
                                    <asp:Button ID="Next" CssClass="Button" runat="server" OnClick="Next_Click" Text="Далі"/>
                                      
                                    <div id="Conclude">
                                        <a class ="BigText" style="font-weight:bold;">Логічне виведення:</a> <asp:Label ID="Label1" runat="server" class ="BigText"></asp:Label>
                                    </div>
                                      
                                    <asp:Panel ID="PanelToAddDynamicTextBox" runat="server"></asp:Panel>
                                    <asp:Button ID="ReadyButton" CssClass="BigButton" runat="server" OnClick="Ready_Click" Text="Провести виведення" />
                                    </div>
                                    <hr />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                    </ContentTemplate>
                </cc1:TabPanel>
            </cc1:TabContainer>
             
            <div id="ListOfRules">
                <asp:UpdatePanel runat="server" ID="UpdateRules" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:ImageButton ID="EditRulesImageButton" Width="3%" Height="3%" CssClass="Pencil" runat="server" ImageUrl="PEN_2.png" OnClick="EditRulesImageButton_Click"/>
                        &nbsp; 
                        <asp:ListBox ID="ListOfRulesBox" runat="server" OnSelectedIndexChanged="ListOfRulesBox_SelectedIndexChanged">
                            <asp:ListItem></asp:ListItem>
                        </asp:ListBox> 
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        <div id="workZone" runat="server" visible ="false">
            <!-- Тут був скрипт -->
        </div>
    </form>
</body>
</html>