<%@ Page Language="C#" AutoEventWireup="true" Inherits="ClientPages.err500" CodeBehind="err500.aspx.cs" %>


<!DOCTYPE html>
<html>
<head>
    <title><% if (AdvantShop.Localization.Culture.CurrentCulture == AdvantShop.Localization.Culture.SupportLanguage.Russian)
               { %>
            Внутренняя ошибка
            <% }
                else
                {%>
            Internal error
            <% } %></title>
    <style type="text/css">
        body {
            font-family: Arial, Helvetica, sans-serif;
            color: #4b4f58;
            font-size: 16px;
        }

        .wrapper {
            width: 80%;
            margin: 0 auto;
        }

        .code-error {
            font-size: 60px;
            font-weight: bold;
            margin-top: 75px;
        }

        .code-descripition {
            font-size: 30px;
            margin-top: 25px;
        }

        .to-do {
        }

            .to-do .head {
                font-weight: bold;
                font-size: 18px;
                margin-top: 60px;
            }

        ul {
            list-style: none;
            padding: 0;
            margin: 0;
        }

            ul li:before {
                content: "- ";
            }

        .to-do .to-do-items li {
            margin: 15px 0;
        }
    </style>
</head>
<body>
    <div class="wrapper">
        <div class="code-error">
            <% if (AdvantShop.Localization.Culture.CurrentCulture == AdvantShop.Localization.Culture.SupportLanguage.Russian)
                { %>
            Ошибка 500
            <% }
                else
                {%>
            Error 500
            <% } %>
        </div>
        <div class="code-descripition">
            <% if (AdvantShop.Localization.Culture.CurrentCulture == AdvantShop.Localization.Culture.SupportLanguage.Russian)
                { %>
            Произошла внутренняя ошибка сервера, ваш запрос не может быть обработан
            <% }
                else
                {%>
            An internal server error has occurred, your request could not be processed.
            <% } %>
        </div>
        <div class="to-do">
            <div class="head">
                <% if (AdvantShop.Localization.Culture.CurrentCulture == AdvantShop.Localization.Culture.SupportLanguage.Russian)
                    { %>
            Попробуйте следующее:
            <% }
                else
                {%>
            Try the following:
            <% } %>
            </div>

            <% if (AdvantShop.Localization.Culture.CurrentCulture == AdvantShop.Localization.Culture.SupportLanguage.Russian)
                { %>
            <ul class="to-do-items">
                <li>Перезагрузите страницу
                </li>
                <li>Вернитесь чуть позже
                </li>
                <li>Вернитесь на <a href="<%= Request.ApplicationPath %>">главную</a>
                </li>
            </ul>
            <% }
                else
                {%>
            <ul class="to-do-items">
                <li>Reload page
                </li>
                <li>Come back later
                </li>
                <li>Return to <a href="<%= Request.ApplicationPath %> ">Home </a>
                </li>
            </ul>
            <% } %>
        </div>
    </div>
</body>
</html>
