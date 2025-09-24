
-- SQL_4.1.10_Part2_Ru

  Update [Settings].[MailFormatType] 
  Set Comment = 'Письмо администратору при заказе в один клик (#ORDER_ID#, #NUMBER#, #NAME#, #COMMENTS#, #PHONE#, #EMAIL#, #ORDERTABLE#, #STORE_NAME#)'
  Where MailFormatTypeID = 16
  
 GO--
 







