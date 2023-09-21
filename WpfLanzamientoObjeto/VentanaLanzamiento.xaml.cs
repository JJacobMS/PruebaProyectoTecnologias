using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfLanzamientoObjeto
{
    /// <summary>
    /// Lógica de interacción para VentanaLanzamiento.xaml
    /// </summary>
    public partial class VentanaLanzamiento : Window
    {
        private List<Jugador> jugadorList;
        private List<Mensaje> mensajeList;
        public VentanaLanzamiento()
        {
            InitializeComponent();

            Jugador jugador = new Jugador();
            jugador.Nickname = "Melus";
            jugador.Correo = "melus@";
            jugador.EnLinea = true;
            jugador.EsHost = true;
            jugador.EstaExpulsado = false;

            Jugador jugador2 = new Jugador();
            jugador2.Nickname = "JJacob";
            jugador2.Correo = "jacob@";
            jugador2.EnLinea = true;
            jugador2.EsHost = false;
            jugador2.EstaExpulsado = false;

            Jugador jugador3 = new Jugador();
            jugador3.Nickname = "jaznoom01";
            jugador3.Correo = "jazmin@";
            jugador3.EnLinea = true;
            jugador3.EsHost = false;
            jugador3.EstaExpulsado = false;

            jugadorList = new List<Jugador>();
            jugadorList.Add(jugador);
            jugadorList.Add(jugador2);
            jugadorList.Add(jugador3);

            Mensaje mensaje1 = new Mensaje();
            mensaje1.AutorMensaje = jugador;
            mensaje1.TextoMensaje = "Hola jugadores";

            Mensaje mensaje2 = new Mensaje();
            mensaje2.AutorMensaje = jugador2;
            mensaje2.TextoMensaje = "Buenas";

            mensajeList = new List<Mensaje> { mensaje1, mensaje2 };


            dtGridJugadores.ItemsSource = jugadorList;
            Console.WriteLine("eas");

            lstBoxChat.ItemsSource = mensajeList;
        }


        private void ClicExpulsar(object sender, RoutedEventArgs e)
        {
            Button botonExpulsado = sender as Button;
            botonExpulsado.IsEnabled = false;
            Jugador jugadorExpulado = botonExpulsado.CommandParameter as Jugador;
            jugadorExpulado.EnLinea = false;
            jugadorExpulado.EstaExpulsado = true;
            dtGridJugadores.Items.Refresh();
        }
    }

    public class Jugador
    {
        public string Nickname { get; set; }
        public string Correo { get; set; }
        public bool EsHost { get; set; }
        public bool EnLinea { get; set; }
        public bool EstaExpulsado { get; set; }
    }

    public class Mensaje
    {
        public Jugador AutorMensaje { get; set; }
        public string TextoMensaje { get; set; }
    }
}
