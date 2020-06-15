﻿/*
 * Created by SharpDevelop.
 * User: David Gutierrez
 * Date: 14/06/2020
 * Time: 05:38 p. m.
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using System.Drawing;

namespace CelularAutomaton {

	public class Canvas {
		//azul pastel (120, 120, 243)
		//rojo pastel (243, 120, 120)
		//verde pastel(120, 243, 120)
		static private Pen pen = new Pen(Color.Black);//lapiz negro
		static private Brush b = new SolidBrush(Color.FromArgb(0, 211, 242));//azul claro
		static private Brush b2= new SolidBrush(Color.FromArgb(175, 211, 242));//azul sobr rojo
		static private Brush b3= new SolidBrush(Color.FromArgb(243, 69, 69));//rojo claro
		static private Brush b4= new SolidBrush(Color.White);//blanco
		private Bitmap bmpBackGroundVisible;
		private Bitmap bmpBackGroundInvisible;
		private Bitmap bmpForeGround;
		private int widthCanvas;
		private int heightCanvas;
		private int row;
		private int column;
		private int width;
		private int height;
		private int[,] matriz;
		
		public Canvas(int widthCanvas, int heightCanvas, int row, int column) {
			this.width = widthCanvas/row;
			this.height= heightCanvas/column;
			this.row	= row;
			this.column	= column;
			this.widthCanvas	= widthCanvas;
			this.heightCanvas	= heightCanvas;
			bmpBackGroundVisible	= new Bitmap(widthCanvas, heightCanvas);
			bmpBackGroundInvisible	= new Bitmap(widthCanvas, heightCanvas);
			bmpForeGround			= new Bitmap(widthCanvas, heightCanvas);
			matriz = new int[column, row];
		}
		
		public Bitmap BackGroundVisible		{ get { return bmpBackGroundVisible; } }
		public Bitmap BackGroundInvisible	{ get { return bmpBackGroundInvisible; } }
		public Bitmap ForeGround			{ get { return bmpForeGround; } }
		public int Row	{ get {  return row; } set { row = value; width = widthCanvas/row; }}
		public int Column	{ get { return column; } set { column = value; height = heightCanvas/column;  }}
		
		
		public void resetMatriz() {
			matriz = new int[column, row];
		}
		
		public void drawMatriz() {
			Graphics g = Graphics.FromImage(bmpBackGroundVisible);
			int x = widthCanvas/row;
			int y = heightCanvas/column;
			for(int i = 0;i <= row; i++) {
				for(int j = 0; j <= column; j++) {
					g.DrawLine(pen, x*i, y*j, x*i, x*row);
					g.DrawLine(pen, x*i, y*j, y*column , y*j);
				}
			}
			g.Dispose();
		}
		
		public void fillCells() {
			//rellenar las celdas con cada celula
			Graphics g = Graphics.FromImage(bmpBackGroundVisible);
			
			for(int y = 0; y < column; y++) {
				for(int x = 0; x < row; x++) {
					//dibujar matriz
					if(matriz[y,x] == 1) {
						g.FillRectangle(b3, x*width+1, y*height+1, width-1, height-1);
					}
				}
			}
			g.Dispose();
		}
		
		public void drawCell(MouseEventArgs e) {
			Graphics g = Graphics.FromImage(bmpBackGroundVisible);
			if(e.X/width < row && e.Y/height < column) {
				if(e.Button == MouseButtons.Left) {
					g.FillRectangle(b3, e.X/width*width+1, e.Y/height*height+1, width-1, height-1);
						matriz[e.Y/height, e.X/width] = 1;
				} else if(e.Button ==  MouseButtons.Right) {
					g.FillRectangle(b4,e.X/width*width+1, e.Y/height*height+1, width-1, height-1);
						matriz[e.Y/height, e.X/width] = 0;
				}
			}
			
			g.Dispose();
		}
		
		public void hoverCell(MouseEventArgs e) {
			Graphics g = Graphics.FromImage(bmpForeGround);
			g.Clear(Color.Transparent);
			if(bmpBackGroundVisible.GetPixel(e.X, e.Y).ToArgb().Equals(Color.FromArgb(243, 69, 69).ToArgb())) {
				g.FillRectangle(b2,e.X/width*width+1, e.Y/height*height+1, width-1, height-1);
			} else {
				g.FillRectangle(b,e.X/width*width+1, e.Y/height*height+1, width-1, height-1);
			}
			g.Dispose();
		}
		
		public String save(String description) {
			int rowsText = 1;
			foreach(char c in description) {
				if(c == '\n') { rowsText++; }
			}
			String str = row.ToString() + "," + column.ToString() + "," + rowsText +"\n";
			str +=  description + "\n";
			for(int y = 0; y < column; y++) {
				for(int x = 0; x < row; x++) {
					str += matriz[y,x];
					
					if(x != row-1) { str += ","; }
				}
				if(y != column-1) { str += "\n"; }
			}
			return str;
		}
		
		public String setDescripcion(String file) {
			//prima fila contiene (ancho, alto, cantidad de columnas de texto)
			//columnas con texto
			//matriz conformada por (ancho y alto)
			//regresar texto
			int rowActual = 0;
			String descripcion = "";
			String[] rowsFile = file.Split('\n');
			int columnsDescripcion = 0;
			
			foreach(String r in rowsFile) {
				if(rowActual == 0) {
					//obtengo primera fila
					String[] values = r.Split(',');
					Row = Int32.Parse(values[0]);
					Column = Int32.Parse(values[1]);
					columnsDescripcion = Int32.Parse(values[2]);
					//inicializar matriz
					matriz = new int[column, row];
				} else if(rowActual <= columnsDescripcion) {
					//obtengo la descripcion
					descripcion += r;
					if(rowActual < columnsDescripcion) {
						//evita generar un salto al final del documento
						descripcion += "\n";
					}
				}else {
					//obtengo la matriz
					//formato de entrada...
					//0,1,0,1,0,1,0,0,1,0
					int x = 0;
					int y = rowActual - (columnsDescripcion+1);
					String[] data = r.Split(',');
					foreach(String col in data) {
						matriz[y,x] = Int32.Parse(col);
						x++;
					}
				}
				
				rowActual++;
			}
			return descripcion;
		}
		
		public void clean() {
			Graphics g = Graphics.FromImage(bmpBackGroundVisible);
			g.Clear(Color.Transparent);
			g.Dispose();
		}
		
		
	}
}