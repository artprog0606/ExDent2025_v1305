
-- SQL_4.1.5_Part2_Ru

  Update [Settings].[MailFormatType] 
  Set Comment = 'Письмо при оформнении нового заказа (#CUSTOMERCONTACTS#, #SHIPPINGMETHOD#, #PAYMENTTYPE#, #ORDERTABLE#, #CURRENTCURRENCYCODE#, #TOTALPRICE#, #COMMENTS#, #EMAIL#, #ORDER_ID#, #NUMBER#, #BILLING_LINK#)'
  Where MailFormatTypeID = 3
  
  GO--
  
  Update [Settings].[MailFormatType] 
  Set Comment = 'Письмо со сылкой на оплату заказа (#ORDER_ID#, #FIRSTNAME#, #BILLING_LINK#, #ORDERTABLE#, #CUSTOMERCONTACTS#, #STORE_NAME#)'
  Where MailFormatTypeID = 17





