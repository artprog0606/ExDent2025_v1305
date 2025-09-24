#--------------------------------------------------
# Project: AdvantShop.NET
# Web site: https:\\www.advantshop.net
#--------------------------------------------------

function FormatErrors($Errors)
{
    if ($Errors.Count -ne 0)
    {
        Write-Host "!---!"
        Write-Host $PS_secondLevelLog "Total errors:", $Errors.Count
        Write-Host "!---!"
        foreach ($error in $Errors)
        {
            if ($null -ne $error.Exception.InnerException)
            {
                Write-Host $PS_secondLevelLog $error.Exception.InnerException.ToString()
            }
            else
            {
                Write-Host $PS_secondLevelLog $error.Exception.ToString()
            }
            Write-Host "---"
        }
    }
}

Export-ModuleMember -Function  *
