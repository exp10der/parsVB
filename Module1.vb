Imports HtmlAgilityPack
Imports System.Net.Http
Imports VBPARSER.ConsoleApplication1

Module Module1

    Sub Main()

        Const base As String = "http://www.cds.spb.ru"
        Dim str As String = String.Empty
        Using client = New HttpClient()
            str = client.GetStringAsync("http://www.cds.spb.ru/novostroiki-peterburga/").Result
        End Using

        Dim doc As New HtmlDocument()
        doc.LoadHtml(str)


        '#Region "tmp"
        Dim q = doc.DocumentNode.SelectNodes("//a[@class='b-object ']").[Select](Function(n)
                                                                                     Dim title_tmp = n.Element("header").Element("p").Element("em").InnerText

                                                                                     Return New With { _
                                                                                         Key .url = base & n.GetAttributeValue("href", ""), _
                                                                                         Key .title = title_tmp _
                                                                                     }

                                                                                 End Function).ToList()
        '#End Region


        ' С текст сканером
        ' 
        Dim result = doc.DocumentNode.SelectNodes("//a[@class='b-object ']").[Select](Function(n)
                                                                                          Dim title_tmp = n.Element("header").Element("h2").InnerText.Trim()
                                                                                          Dim scan = New TextScanner(title_tmp)
                                                                                          scan.Skip("&laquo;")
                                                                                          Return New With { _
                                                                                              Key .url = base & n.GetAttributeValue("href", ""), _
                                                                                              Key .title = scan.ReadTo("&raquo;") _
                                                                                          }

                                                                                      End Function).ToList()

        Dim list = New List(Of String)(result.[Select](Function(n) n.title))

        ' TODO Дома которые сданны лежат в классе "b-object b-object_finish"


    End Sub

End Module
