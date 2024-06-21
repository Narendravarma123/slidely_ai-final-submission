Imports System.Diagnostics
Imports System.Net.Http
Imports System.Text

Public Class CreateSubmissionForm
    Private stopwatch As New Stopwatch()
    Private WithEvents timer As New Timer()

    Public Sub New()
        InitializeComponent()
        InitializeTimer()

        ' Enable KeyPreview to allow form to receive key events before controls
        Me.KeyPreview = True
    End Sub

    Private Sub InitializeTimer()
        timer.Interval = 1000 ' Interval in milliseconds (1 second)
        AddHandler timer.Tick, AddressOf Timer_Tick
    End Sub

    Private Sub Timer_Tick(sender As Object, e As EventArgs)
        lblStopwatch.Text = stopwatch.Elapsed.ToString("hh\:mm\:ss")
    End Sub

    Private Sub btnStopwatch_Click(sender As Object, e As EventArgs) Handles btnStopwatch.Click
        ToggleStopwatch()
    End Sub

    Private Sub ToggleStopwatch()
        If stopwatch.IsRunning Then
            stopwatch.Stop()
            timer.Stop() ' Stop the timer when stopwatch is paused
            btnStopwatch.Text = "Resume"
        Else
            stopwatch.Start()
            timer.Start() ' Start the timer when stopwatch is running
            btnStopwatch.Text = "Pause"
        End If
    End Sub

    Private Async Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        ' Create timestamp
        Dim timestamp As DateTime = DateTime.UtcNow

        ' Create a new Submission object
        Dim submission As New Submission(txtName.Text, txtEmail.Text, txtPhone.Text, txtGitHub.Text, lblStopwatch.Text, timestamp)

        ' Send submission data to backend
        Dim success As Boolean = Await SendSubmissionToBackend(submission)

        If success Then
            ' Show submission saved message
            MessageBox.Show("Submission saved successfully!")

            ' Optionally, clear input fields and reset stopwatch
            txtName.Text = ""
            txtEmail.Text = ""
            txtPhone.Text = ""
            txtGitHub.Text = ""
            stopwatch.Reset()
            lblStopwatch.Text = "00:00:00"

            ' Optionally, open ViewSubmissionsForm to display submissions
            Dim viewSubmissionsForm As New ViewSubmissionsForm()
            viewSubmissionsForm.Show()
        Else
            MessageBox.Show("Failed to save submission. Please try again.")
        End If
    End Sub

    Private Async Function SendSubmissionToBackend(submission As Submission) As Task(Of Boolean)
        Try
            Using client As New HttpClient()
                client.BaseAddress = New Uri("http://localhost:3000") ' Replace with your server URL
                client.DefaultRequestHeaders.Accept.Clear()
                client.DefaultRequestHeaders.Accept.Add(New Headers.MediaTypeWithQualityHeaderValue("application/json"))

                ' Create content to send as JSON
                Dim json As String = $"{{""Name"":""{submission.Name}"",""Email"":""{submission.Email}"",""Phone"":""{submission.Phone}"",""GitHub"":""{submission.GitHub}"",""Time"":""{submission.Time}""}}"
                Dim content As New StringContent(json, Encoding.UTF8, "application/json")

                ' Send POST request
                Dim response As HttpResponseMessage = Await client.PostAsync("/submit", content)

                ' Check response status
                Return response.IsSuccessStatusCode
            End Using
        Catch ex As Exception
            Console.WriteLine("Error sending submission to backend: " & ex.Message)
            Return False
        End Try
    End Function

    Private Sub CreateSubmissionForm_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.S Then
            btnSubmit.PerformClick()
        ElseIf e.Control AndAlso e.KeyCode = Keys.T Then
            ToggleStopwatch()
        End If
    End Sub

    Private Sub CreateSubmissionForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Ensure timer is stopped and resources are released when form closes
        If stopwatch.IsRunning Then
            stopwatch.Stop()
        End If
        timer.Dispose()
    End Sub

    Private Sub txtName_TextChanged(sender As Object, e As EventArgs) Handles txtName.TextChanged
        ' You can handle text changed events here if needed
    End Sub

    Private Sub lblStopwatch_TextChanged(sender As Object, e As EventArgs) Handles lblStopwatch.TextChanged

    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click

    End Sub
End Class

Public Class Submission
    Public Property Name As String
    Public Property Email As String
    Public Property Phone As String
    Public Property GitHub As String
    Public Property Time As String ' Assuming this is the stopwatch time string
    Public Property Timestamp As DateTime ' Ensure the Timestamp property matches the format in JSON
    Public Property Id As String


    Public Sub New()
        ' Default constructor
    End Sub

    Public Sub New(name As String, email As String, phone As String, github As String, time As String, timestamp As DateTime)
        Me.Name = name
        Me.Email = email
        Me.Phone = phone
        Me.GitHub = github
        Me.Time = time
        Me.Timestamp = timestamp
        Me.Id = Id

    End Sub
End Class