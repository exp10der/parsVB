Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Http
Imports System.Text.RegularExpressions
Imports System.Runtime.CompilerServices
Imports HtmlAgilityPack


Module Module1

    Sub Main()


        ' "Получаем все сылки"
        Const base As String = "http://www.cds.spb.ru"

        Dim html As String
        Using client = New HttpClient()
            html = client.GetStringAsync("http://www.cds.spb.ru/novostroiki-peterburga/").Result
        End Using

        Dim doc = New HtmlDocument()
        doc.LoadHtml(html)

        ' Заполняем Первые 3 свойства

        ' Вот тут 3
        Dim realState As List(Of RealEstate) = doc.DocumentNode.SelectNodes(".//*[@id='outer']/div[2]/div/table/tr/td[2]/div[2]/a").AsParallel().[Select](Function(item)
                                                                                                                                                              Dim titleTmp = item.SelectSingleNode("header/h2")
                                                                                                                                                              Dim shortAddressTmp = item.SelectSingleNode("header/p/em")
                                                                                                                                                              Dim uriTmp = item.GetAttributeValue("href", "")
                                                                                                                                                              Return New RealEstate() With { _
                                                                                                                                                                   .title = If(titleTmp IsNot Nothing, titleTmp.InnerText.Trim().FixString(), ""), _
                                                                                                                                                                  .shortAddress = If(shortAddressTmp IsNot Nothing, shortAddressTmp.InnerText.Trim().FixString(), ""), _
                                                                                                                                                                   .Uri = If(New Uri(uriTmp, UriKind.RelativeOrAbsolute).IsAbsoluteUri, uriTmp, base & Convert.ToString(uriTmp)) _
                                                                                                                                                              }

                                                                                                                                                          End Function).ToList()
        ' Удаляем Последнии квартиры в сданных домах
        realState.Remove(realState.Last())
        '

        ' В будущем наверно надо будет сделать список сущностей корые надо будет фиксить
        ' TODO ЭТО ЛИ НЕ ЧУДО?
        Dim urlForFix = realState.Last().Uri
        realState.Remove(realState.Last())
        Using client = New HttpClient()
            html = client.GetStringAsync(urlForFix).Result
        End Using
        doc.LoadHtml(html)


        ' Вот тут 3
        realState.AddRange(doc.DocumentNode.SelectNodes(".//*[@id='outer']/div[2]/div/table/tr/td[2]/div[2]/a").AsParallel().[Select](Function(item)
                                                                                                                                          Dim titleTmp = item.SelectSingleNode("header/h2")
                                                                                                                                          Dim shortAddressTmp = item.SelectSingleNode("header/p/em")
                                                                                                                                          Dim uriTmp = item.GetAttributeValue("href", "")
                                                                                                                                          Return New RealEstate() With { _
                                                                                                                                               .title = If(titleTmp IsNot Nothing, titleTmp.InnerText.Trim().FixString(), ""), _
                                                                                                                                               .shortAddress = If(shortAddressTmp IsNot Nothing, shortAddressTmp.InnerText.Trim().FixString(), ""), _
                                                                                                                                              .Uri = If(New Uri(uriTmp, UriKind.RelativeOrAbsolute).IsAbsoluteUri, uriTmp, base & Convert.ToString(uriTmp)) _
                                                                                                                                          }

                                                                                                                                      End Function).ToList())



        ' Узнаем корпуса и uri уже на квартиры в домах

        ' Если равно Null то делаем вывод то текущий юрл уже привел нас где список квартир!
        ' Т.е на текущей страницы уже идет список квартир

        ' Тут лежит Название корпуса
        'urlsite
        ' А вот если нулл то надо сохранить Исходный код для дальнейшего парсинга наверно надо!
        realState.AsParallel().[Select](Function(n)
                                            n.HousingEstates = New List(Of HousingEstate)()
                                            Dim site As String
                                            Using client = New HttpClient()
                                                site = client.GetStringAsync(n.Uri).Result
                                            End Using
                                            Dim htmldoc = New HtmlDocument()
                                            htmldoc.LoadHtml(site)
                                            Dim housing = htmldoc.DocumentNode.SelectNodes(".//*[@id='outer']/div[2]/div/table/tr/td[2]/div/div/div[2]/a")
                                            If housing IsNot Nothing Then
                                                n.HousingEstates = housing.[Select](Function(x)
                                                                                        Dim tt = x.SelectSingleNode("div/span")
                                                                                        Dim urlsite = x.GetAttributeValue("href", "")
                                                                                        Return New HousingEstate() With { _
                                                                                             .NameHousing = tt.InnerText.Trim(), _
                                                                                             .Uri = If(New Uri(urlsite, UriKind.RelativeOrAbsolute).IsAbsoluteUri, urlsite, base & Convert.ToString(urlsite)) _
                                                                                        }

                                                                                    End Function).ToList()
                                            Else
                                                n.htmlcode = site
                                            End If
                                            Return n

                                        End Function).ToList()



        Dim tmp As New List(Of HousingEstate)()
        For Each item As RealEstate In realState
            If item.HousingEstates.Count <> 0 Then
                For Each housingEstate As HousingEstate In item.HousingEstates

                    tmp.Add(New HousingEstate() With { _
                         .NameHousing = item.title + " " + item.shortAddress + " " + housingEstate.NameHousing, _
                         .Uri = housingEstate.Uri _
                    })
                Next
            Else

                tmp.Add(New HousingEstate() With { _
                     .NameHousing = item.title + " " + item.shortAddress, _
                     .Uri = item.Uri _
                })

            End If
        Next

        '''/ Для теста результат!
        For Each housingEstate As HousingEstate In tmp
            Console.WriteLine(housingEstate.NameHousing)
            Console.WriteLine(housingEstate.Uri)
            Console.WriteLine()
        Next


        Dim result As List(Of ImportHouseInfo) = tmp.[Select](Function(n) New ImportHouseInfo() With { _
             .HouseName = n.NameHousing _
        }).ToList()

































        'Const base As String = "http://www.cds.spb.ru"

        'Dim html As String
        'Using client = New HttpClient()
        '    html = client.GetStringAsync("http://www.cds.spb.ru/novostroiki-peterburga/").Result
        'End Using

        'Dim doc = New HtmlDocument()
        'doc.LoadHtml(html)

        '' Заполняем Первые 3 свойства


        'Dim realState As List(Of RealEstate) = doc.DocumentNode.SelectNodes(".//*[@id='outer']/div[2]/div/table/tr/td[2]/div[2]/a").AsParallel().[Select](Function(item)
        '                                                                                                                                                      Dim titleTmp = item.SelectSingleNode("header/h2")
        '                                                                                                                                                      Dim shortAddressTmp = item.SelectSingleNode("header/p/em")
        '                                                                                                                                                      Dim uriTmp = item.GetAttributeValue("href", "")
        '                                                                                                                                                      ' Вот тут 3
        '                                                                                                                                                      Return New RealEstate() With { _
        '                                                                                                                                                           .title = If(titleTmp IsNot Nothing, titleTmp.InnerText.Trim().FixString(), ""), _
        '                                                                                                                                                           .shortAddress = If(shortAddressTmp IsNot Nothing, shortAddressTmp.InnerText.Trim().FixString(), ""), _
        '                                                                                                                                                          .Uri = If(New Uri(uriTmp, UriKind.RelativeOrAbsolute).IsAbsoluteUri, uriTmp, base & Convert.ToString(uriTmp)) _
        '                                                                                                                                                      }

        '                                                                                                                                                  End Function).ToList()


        '' Узнаем корпуса и uri уже на квартиры в домах
        'realState.AsParallel().[Select](Function(n)
        '                                    n.HousingEstates = New List(Of HousingEstate)()
        '                                    Dim site As String
        '                                    Using client = New HttpClient()
        '                                        site = client.GetStringAsync(n.Uri).Result
        '                                    End Using

        '                                    Dim htmldoc = New HtmlDocument()
        '                                    htmldoc.LoadHtml(site)

        '                                    ' Если равно Null то делаем вывод то текущий юрл уже привел нас где список квартир!
        '                                    ' Т.е на текущей страницы уже идет список квартир
        '                                    Dim housing = htmldoc.DocumentNode.SelectNodes(".//*[@id='outer']/div[2]/div/table/tr/td[2]/div/div/div[2]/a")
        '                                    If housing IsNot Nothing Then
        '                                        n.HousingEstates = housing.[Select](Function(x)
        '                                                                                ' Тут лежит Название корпуса
        '                                                                                Dim tt = x.SelectSingleNode("div/span")
        '                                                                                Dim urlsite = x.GetAttributeValue("href", "")
        '                                                                                Return New HousingEstate() With { _
        '                                                                                     .NameHousing = tt.InnerText.Trim(), _
        '                                                                                     .Uri = urlsite _
        '                                                                                }

        '                                                                            End Function).ToList()
        '                                        ' А вот если нулл то надо сохранить Исходный код для дальнейшего парсинга наверно надо!
        '                                    Else
        '                                        n.htmlcode = site
        '                                    End If
        '                                    Return n

        '                                End Function).ToList()

        'realState.Last().title = "Последнии квартиры в сданных домах!"

        'For Each realEstate As RealEstate In realState
        '    Console.WriteLine(realEstate.shortAddress)
        '    Console.WriteLine("  " + realEstate.title)
        '    For Each estate As HousingEstate In realEstate.HousingEstates


        '        Console.WriteLine(vbTab + estate.NameHousing)
        '    Next
        '    Console.WriteLine()
        'Next

        'Dim list As List(Of ImportHouseInfo) = realState.[Select](Function(n) New ImportHouseInfo() With { _
        '     .strHouseName = n.title _
        '}).ToList()


        'Dim full = realState.SelectMany(Function(n) n.HousingEstates.[Select](Function(x) New ImportHouseInfo() With { _
        '        .strHouseName = n.title + " " + n.shortAddress + " " + x.NameHousing _
        '    })).ToList()
    End Sub
End Module


''' <summary>
''' Недвижимость
''' </summary>
Class RealEstate
    ''' <summary>
    ''' Короткое название
    ''' </summary>
    Public Property title() As String
        Get
            Return m_title
        End Get
        Set(value As String)
            m_title = value
        End Set
    End Property
    Private m_title As String
    ''' <summary>
    ''' Адрес
    ''' </summary>
    Public Property shortAddress() As String
        Get
            Return m_shortAddress
        End Get
        Set(value As String)
            m_shortAddress = value
        End Set
    End Property
    Private m_shortAddress As String
    ''' <summary>
    ''' Сылка на Жилой комплекс
    ''' </summary>
    Public Property Uri() As String
        Get
            Return m_Uri
        End Get
        Set(value As String)
            m_Uri = value
        End Set
    End Property
    Private m_Uri As String

    ''' <summary>
    ''' Тут идет вторая вложенность страницы где название корпусов
    ''' некотрые страницы могут иметь сразу сылку на табличку для квартир
    ''' </summary>
    Public Property HousingEstates() As List(Of HousingEstate)
        Get
            Return m_HousingEstates
        End Get
        Set(value As List(Of HousingEstate))
            m_HousingEstates = value
        End Set
    End Property
    Private m_HousingEstates As List(Of HousingEstate)

    ''' <summary>
    ''' Если тут есть исходный код то данный объект во второй вложености имеет уже табличку с квартирами
    ''' </summary>
    Public Property htmlcode() As String
        Get
            Return m_htmlcode
        End Get
        Set(value As String)
            m_htmlcode = value
        End Set
    End Property
    Private m_htmlcode As String

End Class

''' <summary>
''' Жилой коплекс
''' </summary>
Class HousingEstate
    ' Название корпуса
    Public Property NameHousing() As String
        Get
            Return m_NameHousing
        End Get
        Set(value As String)
            m_NameHousing = value
        End Set
    End Property
    Private m_NameHousing As String
    ' Сылка на таблицу
    Public Property Uri() As String
        Get
            Return m_Uri
        End Get
        Set(value As String)
            m_Uri = value
        End Set
    End Property
    Private m_Uri As String
End Class


Module StringExtensions
    <Extension()> _
    Public Function FixString(ByVal str As String)
        Return Regex.Replace(str, "[a-zA-z&;<>]*", "")
    End Function
End Module


Class ImportHouseInfo
    Public strHouseName As String
    Public Property HouseName() As String
        Get
            Return strHouseName
        End Get
        Set(value As String)
            strHouseName = value
        End Set
    End Property
End Class
