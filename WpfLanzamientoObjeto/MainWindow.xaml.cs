using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfLanzamientoObjeto
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _temporizadorJuego;
        private readonly DispatcherTimer _temporizadorVelocidad;
        private readonly DispatcherTimer _temporizadorLinea;
        private readonly DispatcherTimer _temporizadorDado;

        private LineaMovimiento _lineaTablero;
        private Peon peonNuestro;
        private List<Peon> peonesTablero;
        private Dado _dadoNuestro;

        private const int DISTANCIA_MINIMA_LANZAMIENTO = 1;

        private const int _margen = 5;
        public MainWindow()
        {
            InitializeComponent();
            cnvEspacioJuego.Focus();
            _temporizadorJuego = new DispatcherTimer();
            _temporizadorJuego.Tick += EventosJuegos;
            _temporizadorJuego.Interval = TimeSpan.FromMilliseconds(20);
            _temporizadorJuego.Start();

            _temporizadorVelocidad = new DispatcherTimer();
            _temporizadorVelocidad.Tick += EventosVelocidadPeones;
            _temporizadorVelocidad.Interval = TimeSpan.FromSeconds(3);
            _temporizadorVelocidad.Start();

            _temporizadorLinea = new DispatcherTimer();
            _temporizadorLinea.Tick += EventosVelocidadLinea;
            _temporizadorLinea.Interval = TimeSpan.FromMilliseconds(10);

            _dadoNuestro = new Dado(imgDado);

            _temporizadorDado = new DispatcherTimer();
            _temporizadorDado.Tick += EventoDados;
            _temporizadorDado.Interval = TimeSpan.FromMilliseconds(200);
            _temporizadorDado.Start();

            peonNuestro = new Peon (ellPeon);
            peonesTablero = new List<Peon> 
            {
                new Peon (ellPeonOtro), new Peon (ellPeonEnemigo),peonNuestro
            };
        }

        private void EventoDados(object sender, EventArgs e)
        {
            _dadoNuestro.CambiarNumeroDado();
        }

        private void EventosVelocidadLinea(object sender, EventArgs e)
        {
            _lineaTablero.SiguientePosicion();
        }

        private void EventosVelocidadPeones(object sender, EventArgs e)
        {
            foreach (Peon peonAMover in peonesTablero)
            {
                if (peonAMover.VelocidadHorizontal > 0 || peonAMover.VelocidadVertical > 0)
                {
                    peonAMover.DisminuirVelocidad();
                }
            }
        }

        private void EventosJuegos(object sender, EventArgs e)
        {
            foreach (Peon peonAMover in peonesTablero)
            {
                RealizarMovimiento(peonAMover);
            }
        }

        private void ClicLanzar(object sender, RoutedEventArgs e)
        {
            _temporizadorLinea.Stop();
            peonNuestro.CambiarDireccionVertical(Direccion.Arriba);

            if (_lineaTablero.CoordenadaY != _lineaTablero.Longitud)
            {
                if (_lineaTablero.LadoLineaX == -1)
                {
                    peonNuestro.CambiarDireccionHorizontal(Direccion.Izquierda);
                }
                else
                {
                    peonNuestro.CambiarDireccionHorizontal(Direccion.Derecha);
                }
            }
            int multiplicadorDados = _dadoNuestro.NumeroDado;
            (int distanciaVertical, int distanciaHorizontal) = CalcularDistanciasConBaseMultiplicador(multiplicadorDados, _lineaTablero.RegresarAnguloFormado());
            peonNuestro.VelocidadVertical = distanciaVertical;
            peonNuestro.VelocidadHorizontal = distanciaHorizontal;

        }

        public (int, int) CalcularDistanciasConBaseMultiplicador(int multiplicador, double anguloFormado)
        {
            double anguloValorAbsoluto = Math.Abs(anguloFormado);

            if (anguloValorAbsoluto == 90)
            {
                return (DISTANCIA_MINIMA_LANZAMIENTO * multiplicador, 0);
            }

            if (anguloValorAbsoluto == 0)
            {
                return (0, DISTANCIA_MINIMA_LANZAMIENTO * multiplicador);
            }

            int distanciaVertical = Convert.ToInt32(DISTANCIA_MINIMA_LANZAMIENTO * multiplicador);
            int distanciaHorizontal = Convert.ToInt32(distanciaVertical / Math.Tan(anguloValorAbsoluto));
            return (distanciaVertical, distanciaHorizontal);
        }


        private void ClicIniciar(object sender, RoutedEventArgs e)
        {
            _temporizadorDado.Stop();
            _lineaTablero = new LineaMovimiento(lnAnguloLanzamiento);
            _temporizadorLinea.Start();
        }

        private void RealizarMovimiento(Peon peonAMover)
        {
            
            if (peonAMover.EnMovimientoDerecha)
            {
                HacerMovimientoDerecha(peonAMover);
            }

            if (peonAMover.EnMovimientoIzquierda)
            {
                HacerMovimientoIzquierda(peonAMover);
            }

            if (peonAMover.EnMovimientoArriba)
            {
                HacerMovimientoArriba(peonAMover);
            }

            if (peonAMover.EnMovimientoAbajo)
            {
                HacerMovimientoAbajo(peonAMover);
            }

            foreach (Peon peon in peonesTablero)
            {
                if (peon != peonAMover)
                {
                    if (IntersectaConOtroCirculo(peonAMover, peon) && (peonAMover.DireccionHorizontal != Direccion.Ninguna || peonAMover.DireccionVertical != Direccion.Ninguna))
                    {
                        CambiarDireccion(peonAMover, peon);
                    }
                }
            }
            
        }

        private bool IntersectaConOtroCirculo(Peon peonActual, Peon peonEnemigo)
        {
            //Si la distancia entre los centros es menor que la suma de los radios, los círculos se intersectan; de lo contrario, no se intersectan.
            double centroXPeonActual = Canvas.GetLeft(peonActual.Figura) + peonActual.Figura.Width / 2;
            double centroYPeonActual = Canvas.GetTop(peonActual.Figura) + peonActual.Figura.Height / 2;

            double centroXPeonEnemigo = Canvas.GetLeft(peonEnemigo.Figura) + peonEnemigo.Figura.Width / 2;
            double centroYPeonEnemigo = Canvas.GetTop(peonEnemigo.Figura) + peonEnemigo.Figura.Height / 2;

            double distancia = Math.Sqrt(Math.Pow(centroXPeonEnemigo - centroXPeonActual, 2) + Math.Pow(centroYPeonEnemigo - centroYPeonActual, 2));

            double radioFiguraActual = peonActual.Figura.Width / 2;
            double radioFiguraEnemiga = peonEnemigo.Figura.Width / 2;

            return distancia < (radioFiguraActual + radioFiguraEnemiga);
        }

        private void CambiarDireccion(Peon peonQueChoco, Peon peonConElQueChocaron)
        {

            if (peonQueChoco.DireccionHorizontal != Direccion.Ninguna)
            {
                if (peonQueChoco.DireccionHorizontal == Direccion.Derecha)
                {
                    peonQueChoco.CambiarDireccionHorizontal(Direccion.Izquierda);
                    peonConElQueChocaron.CambiarDireccionHorizontal(Direccion.Derecha);
                    Canvas.SetLeft(peonConElQueChocaron.Figura, Canvas.GetLeft(peonConElQueChocaron.Figura) + peonQueChoco.VelocidadHorizontal);
                }
                else
                {
                    peonQueChoco.CambiarDireccionHorizontal(Direccion.Derecha);
                    peonConElQueChocaron.CambiarDireccionHorizontal(Direccion.Izquierda);
                    Canvas.SetLeft(peonConElQueChocaron.Figura, Canvas.GetLeft(peonConElQueChocaron.Figura) - peonQueChoco.VelocidadHorizontal);
                }
            }

            if (peonQueChoco.DireccionVertical != Direccion.Ninguna)
            {
                if (peonQueChoco.DireccionVertical == Direccion.Arriba)
                {
                    peonQueChoco.CambiarDireccionVertical(Direccion.Abajo);
                    peonConElQueChocaron.CambiarDireccionVertical(Direccion.Arriba);
                    Canvas.SetTop(peonConElQueChocaron.Figura, Canvas.GetTop(peonConElQueChocaron.Figura) - peonQueChoco.VelocidadVertical);
                }
                else
                {
                    peonQueChoco.CambiarDireccionHorizontal(Direccion.Arriba);
                    peonConElQueChocaron.CambiarDireccionVertical(Direccion.Abajo);
                    Canvas.SetTop(peonConElQueChocaron.Figura, Canvas.GetTop(peonConElQueChocaron.Figura) + peonQueChoco.VelocidadVertical);
                }
            }
            
            peonConElQueChocaron.VelocidadHorizontal += peonQueChoco.VelocidadHorizontal / 2;
            peonQueChoco.VelocidadHorizontal /= 2;
        }

        private void HacerMovimientoAbajo(Peon peonAMover)
        {
            if ((Canvas.GetTop(peonAMover.Figura) + peonAMover.Figura.Height * 2) < Application.Current.MainWindow.Height)
            {
                Canvas.SetTop(peonAMover.Figura, Canvas.GetTop(peonAMover.Figura) + peonAMover.VelocidadVertical);
            }
            else
            {
                peonAMover.VelocidadVertical /= 2;
                if (peonAMover.VelocidadVertical > 0)
                {
                    peonAMover.CambiarDireccionVertical(Direccion.Arriba);
                }
            }
        }

        private void HacerMovimientoArriba(Peon peonAMover)
        {
            if (Canvas.GetTop(peonAMover.Figura) > 0 + _margen)
            {
                Canvas.SetTop(peonAMover.Figura, Canvas.GetTop(peonAMover.Figura) - peonAMover.VelocidadVertical);
            }
            else
            {
                peonAMover.VelocidadVertical /= 2;
                if (peonAMover.VelocidadVertical > 0)
                {
                    peonAMover.CambiarDireccionVertical(Direccion.Abajo);
                }
            }
        }

        private void HacerMovimientoIzquierda(Peon peonAMover)
        {
            if (Canvas.GetLeft(peonAMover.Figura) > 0 + _margen)
            {
                Canvas.SetLeft(peonAMover.Figura, Canvas.GetLeft(peonAMover.Figura) - peonAMover.VelocidadHorizontal);
            }
            else
            {
                peonAMover.VelocidadHorizontal /= 2;
                if (peonAMover.VelocidadHorizontal > 0)
                {
                    peonAMover.CambiarDireccionHorizontal(Direccion.Derecha);
                }
            }
        }

        private void HacerMovimientoDerecha(Peon peonAMover)
        {
            if ((Canvas.GetLeft(peonAMover.Figura) + peonAMover.Figura.Width * 2) < (Application.Current.MainWindow.Width))
            {
                Canvas.SetLeft(peonAMover.Figura, Canvas.GetLeft(peonAMover.Figura) + peonAMover.VelocidadHorizontal);
            }
            else
            {
                peonAMover.VelocidadHorizontal /= 2;
                if (peonAMover.VelocidadHorizontal > 0)
                {
                    peonAMover.CambiarDireccionHorizontal(Direccion.Izquierda);
                }
            }
        }

    }

    public class Peon
    {
        public Ellipse Figura { get; set; }
        public int VelocidadHorizontal { get; set; }
        public int VelocidadVertical { get; set; }
        public bool EnMovimientoDerecha { get; set; }
        public bool EnMovimientoIzquierda { get; set; }
        public bool EnMovimientoArriba { get; set; }
        public bool EnMovimientoAbajo { get; set; }
        public Direccion DireccionHorizontal { get; set; }
        public Direccion DireccionVertical { get; set; }

        public Peon()
        {
        }

        public Peon(Ellipse figura)
        {
            Figura = figura;
            VelocidadHorizontal = 0;
            VelocidadVertical = 0;
            EnMovimientoAbajo = false;
            EnMovimientoArriba = false;
            EnMovimientoDerecha = false;
            EnMovimientoIzquierda = false;
        }

        public void CambiarDireccionHorizontal(Direccion direccionNueva)
        {
            DireccionHorizontal = direccionNueva;
            switch (direccionNueva)
            {
                case Direccion.Izquierda:
                    EnMovimientoIzquierda = true;
                    EnMovimientoDerecha = false;
                    break;
                case Direccion.Derecha:
                    EnMovimientoIzquierda = false;
                    EnMovimientoDerecha = true;
                    break;
                default:
                    break;
            }
        }

        public void CambiarDireccionVertical(Direccion direccionNueva)
        {
            DireccionVertical = direccionNueva;
            switch (direccionNueva)
            {
                case Direccion.Abajo:
                    EnMovimientoAbajo = true;
                    EnMovimientoArriba = false;
                    break;
                case Direccion.Arriba:
                    EnMovimientoAbajo = false;
                    EnMovimientoArriba = true;
                    break;
                default:
                    break;
            }
        }

        public void DisminuirVelocidad()
        {
            VelocidadHorizontal = (VelocidadHorizontal - 1 <= 0) ? 0 : VelocidadHorizontal - 1;
            VelocidadVertical = (VelocidadVertical - 1 <= 0) ? 0 : VelocidadVertical - 1;
            if (VelocidadHorizontal <= 0)
            {
                EnMovimientoDerecha = false;
                EnMovimientoIzquierda = false;
            }

            if (VelocidadVertical <= 0)
            {
                EnMovimientoAbajo = false;
                EnMovimientoArriba = false;
            }
        }
    }

    public class LineaMovimiento
    {
        public int CoordenadaY { get; set; }
        public Line FiguraLinea { get; set; }
        public double Longitud { get; set; }
        public int LadoLineaX { get; set; }
        public int LadoLineaY { get; set; }
        public int IntervalosMovimientos { get; set; }

        public LineaMovimiento()
        {
        }

        public LineaMovimiento(Line linea)
        {
            IntervalosMovimientos = 5;
            LadoLineaX = -1;
            LadoLineaY = 1;
            Longitud = 150;
            CoordenadaY = 0;
            FiguraLinea = linea;
        }

        public void SiguientePosicion()
        {
            if (CoordenadaY <= -Longitud)
            {
                LadoLineaX *= -1;
                LadoLineaY *= -1;
            }
            //iNTENTAR OCUPAR SEN
            CoordenadaY -= IntervalosMovimientos * LadoLineaY;

            double distanciaLineaAlCuadrado = Math.Pow(Longitud,2);
            double coordenadaYAlCuadrado = Math.Pow(CoordenadaY, 2);
            double valorAbsolutoResta = Math.Abs(distanciaLineaAlCuadrado - coordenadaYAlCuadrado);
            FiguraLinea.X2 = LadoLineaX * Math.Sqrt(valorAbsolutoResta);
            FiguraLinea.Y2 = CoordenadaY;

            if (CoordenadaY >= 0)
            {
                LadoLineaY *= -1;
            }
        }

        public double RegresarAnguloFormado()
        {
            double valorAbsolutoCatetoOpuesto = Math.Abs(FiguraLinea.Y2);
            return Math.Asin(valorAbsolutoCatetoOpuesto/Longitud);
        }

    }

    public class Dado
    {
        public int NumeroDado { get; set; }
        public Dictionary<int, BitmapImage> ImagenDadoCorrespondiente { get; set; }
        public Image ImagenDado { get; set; }

        public Dado()
        {
            
        }
        public Dado(Image imagenDado)
        {
            ImagenDado = imagenDado;

            ImagenDadoCorrespondiente = new Dictionary<int, BitmapImage>
            {
                { 1, new BitmapImage(new Uri("pack://application:,,,/Recursos/gato1.png")) },
                { 2, new BitmapImage(new Uri("pack://application:,,,/Recursos/gato2.png")) },
                { 3, new BitmapImage(new Uri("pack://application:,,,/Recursos/gato3.png")) },
                { 4, new BitmapImage(new Uri("pack://application:,,,/Recursos/gato4.png")) },
                { 5, new BitmapImage(new Uri("pack://application:,,,/Recursos/gato5.png")) },
                { 6, new BitmapImage(new Uri("pack://application:,,,/Recursos/gato6.png")) }
            };

            Random rnd = new Random();
            NumeroDado = rnd.Next(5) + 1;

            Console.WriteLine(NumeroDado);
            imagenDado.Source = ImagenDadoCorrespondiente[NumeroDado];
        }

        public void CambiarNumeroDado()
        {
            NumeroDado = (NumeroDado + 1 > 6) ? 1 : NumeroDado + 1;
            ImagenDado.Source = ImagenDadoCorrespondiente[NumeroDado];
        }

    }

    public enum Direccion
    {
        Ninguna, Izquierda, Derecha, Arriba, Abajo
    }
}
