
Insert Into [Settings].[MailFormatType] ([TypeName],[SortOrder],[Comment]) VALUES ('Ссылка на оплату заказа', 160, 'Письмо со сылкой на оплату заказа (#ORDER_ID#,#FIRSTNAME#,#BILLING_LINK#,#ORDERTABLE#,#STORE_NAME#)')

GO--

  Insert Into [Settings].[MailFormat] ([FormatName],[FormatText],[FormatType],[SortOrder],[Enable],[AddDate],[ModifyDate],[FormatSubject]) 
  VALUES('Ссылка на оплату заказа', '#LOGO#<br />
<div style="font-family: Verdana; font-size: 13px;"><strong><span style="color: rgb(0, 103, 162);">Ad</span>Vant<span style="color: rgb(0, 103, 162);">Shop</span> .NET</strong><br />
<span style="color: rgb(153, 153, 153); font-size: 9pt;">Бизнес решение для электронной коммерции</span></div>
<p>Здравствуйте, #FIRSTNAME#!</p>
<p>Для того, чтобы оплатить заказ № #ORDER_ID# перейдите по ссылке:</p>
<p><a href="#BILLING_LINK#">#BILLING_LINK#</a></p>
<p>Содержание заказа:<br />#ORDERTABLE#</p>', 17, 130, 1, Getdate(), Getdate(), 'Сылка на оплату заказа #ORDER_ID# в магазине #STORE_NAME#')

GO--

Update [Settings].[MailFormat] 
	Set FormatText = REPLACE(FormatText, '#PAYMENTTYPE#', '#PAYMENTTYPE# (<a href="#BILLING_LINK#">Изменить способ оплаты</a>)')
  Where FormatType = 3

GO--
