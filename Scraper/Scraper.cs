using System;
using System.Collections.Generic;
using Supremes;
using Supremes.Nodes;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Scraper
{
    public class Scraper
    {
        private const string mainPageUrl = "http://www.kaufland.hr/ponuda/ponuda-pregled.category=01_Meso__perad__kobasice.html";

        public static List<Item> FetchItemsKaufland()
        {
            List<Element> pages = new List<Element>();
            List<Item> ItemsList = new List<Item>();
            int id = 0;

            //Start on the main page, find links to other pages
            Document mainPage = Dcsoup.Parse(new Uri(mainPageUrl), 15000);
            Elements pageButtons = mainPage.Select(".m-accordion__item--level-2");
            Elements pageLinks = pageButtons.Select("a");

            //Parse other pages
            pages = GetPagesFromLinks(pageLinks);

            foreach (Element page in pages)
            {
                Elements offers = page.Select(".m-offer-tile__link");

                foreach (Element offer in offers)
                {
                    string company = null, name, priceString, discountString, image;

                    // Items with title and subtitle -> subtitle = company name, title = product name
                    // Items with only a subtitle ->    subtitle = product name
                    name = offer.Select(".m-offer-tile__title").Text;

                    if (string.IsNullOrEmpty(name))
                        name = offer.Select(".m-offer-tile__subtitle").Text;
                    else
                        company = offer.Select(".m-offer-tile__subtitle").Text;

                    // remove non number characters
                    priceString = Regex.Replace(offer.Select(".a-pricetag__price").Text, "[^.0-9]", "");
                    float.TryParse(priceString, out float price);

                    discountString = Regex.Replace(offer.Select(".a-pricetag__discount").Text, "[^.0-9]", "");
                    Debug.WriteLine(discountString);
                    float.TryParse(discountString, out float discount);

                    image = offer.Select(".a-image-responsive").Attr("src");

                    ItemsList.Add(new Item
                    {
                        ItemId = id++,
                        Name = name,
                        Company = company,
                        Quantity = offer.Select(".m-offer-tile__quantity").Text,
                        Price = price,
                        Discount = discount,
                        ImageUrl = image
                    });
                }  
            }

            return ItemsList;
        }

        private static List<Element> GetPagesFromLinks(Elements pageLinks)
        {
            List<Element> pages = new List<Element>();

            foreach (Element link in pageLinks)
            {
                if (link.Attr("href").Contains("/ponuda/ponuda-pregled.category"))
                {
                    //Debug.WriteLine("href: " + link.Attr("href"));
                    Element page = Dcsoup.Parse(new Uri("https://www.kaufland.hr" + link.Attr("href")), 15000);
                    pages.Add(page);

                    if (link.Attr("href").Contains("/ponuda/ponuda-pregled.category=default_Ostalo.html"))
                        return pages;
                }
            }

            return pages;
        }
    }
}