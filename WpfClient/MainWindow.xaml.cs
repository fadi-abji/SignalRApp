using Microsoft.AspNetCore.SignalR.Client;
using System.Windows;

namespace WpfClient;

public partial class MainWindow : Window
{
    HubConnection connection;
    HubConnection counterConnecton;
    public MainWindow()
    {
        InitializeComponent();
        connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7172/chathub")
            .WithAutomaticReconnect()
            .Build();

        InitializeComponent();
        counterConnecton = new HubConnectionBuilder()
            .WithUrl("https://localhost:7172/counterhub")
            .WithAutomaticReconnect()
            .Build();

        //connection.Reconnected += async (string arg) =>
        //{
        //    await connection.SendAsync("SendMessage", "WPF Client", "Reconnected");
        //};

        connection.Reconnecting += async (sender) =>
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                var newMessage = "Attempting to reconnect...";
                messages.Items.Add(newMessage);
            });
        };

        connection.Reconnected += async (sender) =>
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                var newMessage = "Reconnected to the server";
                messages.Items.Clear();
                messages.Items.Add(newMessage);
            });
        };

        connection.Closed += async (sender) =>
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                var newMessage = "Connection Closed";
                messages.Items.Add(newMessage);
                openConnection.IsEnabled = true;
                sendMessage.IsEnabled = false;
            });
        };
    }

    private async void openConnection_Click(object sender, RoutedEventArgs e)
    {
        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                messages.Items.Add($"{user}: {message}");
            });
        });

        try
        {
            await connection.StartAsync();
            messages.Items.Add("Connection started");
            openConnection.IsEnabled = false;
            sendMessage.IsEnabled = true;
        }
        catch (Exception ex)
        {
            messages.Items.Add(ex.Message);
        }
    }

    private async void sendMessage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await connection.InvokeAsync("SendMessage", "WPF Client", messageInput.Text);
        }
        catch (Exception ex)
        {
            messages.Items.Add(ex.Message);
        }
    }

    private async void openCounter_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (counterConnecton.State == HubConnectionState.Disconnected)
            {
                await counterConnecton.StartAsync();
            }
            openCounter.IsEnabled = false;
        }
        catch (Exception ex)
        {
            messages.Items.Add(ex.Message);
        }
    }

    private async void incrementCouter_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await counterConnecton.InvokeAsync("AddToTotal", "WPF Client", 1);
        }
        catch (Exception ex)
        {
            messages.Items.Add(ex.Message);
        }
    }
}