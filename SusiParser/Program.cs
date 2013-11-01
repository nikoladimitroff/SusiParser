using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SusiParser
{
	class Program
	{
		static string passwordFile = "password.txt";

		enum AspFormValidators
		{
			EventValidation
		}

		static void Main(string[] args)
		{
			SusiParser parser = new SusiParser();
			parser.Login("nikola1", File.ReadAllText(passwordFile).Trim());
			string examPage = parser.GetExamPage();

			HtmlDocument document = new HtmlDocument();
			document.LoadHtml(examPage);


			string xpath = @"//table";////table";//[width=""100%""]";//tr[@class!=""greyType2""]";

			HtmlNodeCollection nodes = document.DocumentNode.SelectNodes(xpath);

			foreach (var node in nodes)
			{
				var children = node.SelectNodes(xpath);
				foreach (var child in children)
				{
					
					Console.WriteLine(child.OuterHtml);
				}
			}

			//HtmlNode current = document.DocumentNode;
			//List<HtmlNode> tables = new List<HtmlNode>();
			//foreach (var node in current.ChildNodes)
			//{
			//	if (node.NodeType == HtmlNodeType.Element && node.
			//}

		}

		static void PrintNodes(HtmlNode node)
		{

			Console.WriteLine(node.OuterHtml);
			foreach (var child in node.ChildNodes)
			{
				PrintNodes(child);
			}
		}
	}
}
