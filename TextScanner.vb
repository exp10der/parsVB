Namespace ConsoleApplication1
    ''' <summary>
    ''' Класс для извлечения нужной информации из разнородного текста
    ''' </summary>
    Public Class TextScanner
        '--- members ---------------------------------------------------------
        Private mText As String
        Private mPosition As Integer
        Private mStartRead As Integer


        '--- public ----------------------------------------------------------
        ''' <summary>
        ''' Конструктор
        ''' </summary>
        ''' <param name="text">Текст для обработки</param>
        Public Sub New(text As String)
            mText = text
            mPosition = 0
            mStartRead = -1
        End Sub
        Public Sub refresh()
            mPosition = 0
        End Sub
        ''' <summary>
        ''' Перемещение указателя в конец текста
        ''' </summary>
        Public Sub GoToEnd()
            mPosition = mText.Length
        End Sub

        ''' <summary>
        ''' Перемещение указателя на начало искомого текста
        ''' </summary>
        ''' <param name="text">Искомый текст</param>
        Public Sub [GoTo](text As String)
            If Not TryGoTo(text) Then
                Throw New Exception("Неверный формат текста")
            End If
        End Sub

        ''' <summary>
        ''' Попытка перемещения указателя на начало искомого текста
        ''' </summary>
        ''' <param name="text">Искомый текст</param>
        ''' <returns>true в случае удачи</returns>
        Public Function TryGoTo(text As String) As Boolean
            Dim p As Integer = mText.IndexOf(text, mPosition)

            If p = -1 Then
                Return False
            End If

            mPosition = p
            Return True
        End Function

        ''' <summary>
        ''' Перемещение указателя за искомый текст
        ''' </summary>
        ''' <param name="text">Искомый текст</param>
        Public Sub Skip(text As String)
            If Not TrySkip(text) Then
                Throw New Exception("Неверный формат текста")
            End If
        End Sub

        ''' <summary>
        ''' Попытка перемещения указателя за искомый текст
        ''' </summary>
        ''' <param name="text">Искомый текст</param>
        ''' <returns>true в случае удачи</returns>
        Public Function TrySkip(text As String) As Boolean
            Dim p As Integer = mText.IndexOf(text, mPosition)

            If p = -1 Then
                Return False
            End If

            mPosition = p + text.Length
            Return True
        End Function

        ''' <summary>
        ''' Начать чтение с текущего места
        ''' </summary>
        Public Sub BeginRead()
            If mPosition = mText.Length Then
                Throw New Exception("Указатель в конце текста")
            End If

            mStartRead = mPosition
        End Sub

        ''' <summary>
        ''' Завершить чтение
        ''' </summary>
        ''' <returns>Прочитанный текст</returns>
        Public Function EndRead() As String
            If mStartRead = -1 Then
                Throw New Exception("Необходимо предварительно начать чтение")
            End If

            Dim r As String = mText.Substring(mStartRead, mPosition - mStartRead)
            mStartRead = -1
            Return r
        End Function

        ''' <summary>
        ''' Прочитать текст с текущей позиции до указанного текста
        ''' </summary>
        ''' <param name="text">Стоп-текст</param>
        ''' <returns>Прочитанный текст</returns>
        Public Function ReadTo(text As String) As String
            BeginRead()
            [GoTo](text)
            Return EndRead()
        End Function
    End Class
End Namespace
