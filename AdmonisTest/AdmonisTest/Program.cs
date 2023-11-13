using System;
using System.Collections.Generic;
using System.Xml;
using static AdmonisTest.AdmonisTest;

namespace AdmonisConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\Users\miriam\Desktop\Admonis\Product\Product.xml";

            List<string> processedVariants = new List<string>();

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.ReadToFollowing("product"))
                {
                    AdmonisProduct product = new AdmonisProduct();
                    string productId = reader.GetAttribute("product-id");
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            // get the AdmonisProduct property value. 
                            switch (reader.Name)
                            {
                                case "display-name":
                                    product.Name = reader.ReadElementContentAsString();
                                    break;
                                case "short-description":
                                    product.Description = reader.ReadElementContentAsString();
                                    break;
                                case "long-description":
                                    product.DescriptionLong = reader.ReadElementContentAsString();
                                    break;
                                case "brand":
                                    product.Brand = reader.ReadElementContentAsString();
                                    break;
                                case "variations":
                                    // The product has variations
                                    processedVariants.Add(productId);
                                    List<AdmonisProductOption> productOptions = ProcessVariant(reader, processedVariants, filePath);
                                    product.Options.Add(productOptions);
                                    PrintProductDetails(product);
                                    break;
                            }
                        }
                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "product")
                        {
                            break;
                        }
                    }
                    if (processedVariants.Count > 0 && !processedVariants.Contains(productId))
                    {
                        // The product does not have variations and has not been processed as a variant, process it as a standalone product
                        PrintProductDetails(product);
                    }
                }
            }
            Console.ReadLine();
        }
        // get the AdmonisProductOption Size and Color
        private static List<AdmonisProductOption> ProcessVariant(XmlReader reader, List<string> processedVariants, string filePath)
        {
            List<AdmonisProductOption> product = new List<AdmonisProductOption>();
            reader.ReadToFollowing("variants");
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "variant")
                    {
                        string variantProductId = reader.GetAttribute("product-id");
                        processedVariants.Add(variantProductId);
                        AdmonisProductOption option = new AdmonisProductOption();

                        // Map fields from XML to AdmonisProductOption
                        option.optionName = variantProductId;
                        XmlReader reader_variant = XmlReader.Create(filePath);
                        while (reader_variant.ReadToFollowing("product"))
                        {
                            if (reader_variant.GetAttribute("product-id") == variantProductId)
                            {
                                if (reader_variant.ReadToFollowing("custom-attributes"))
                                {
                                    // Read custom attributes
                                    while (reader_variant.ReadToFollowing("custom-attribute"))
                                    {
                                        if (!string.IsNullOrEmpty(option.Color) && !string.IsNullOrEmpty(option.Size))
                                            break;
                                        string attributeId = reader_variant.GetAttribute("attribute-id");
                                        if (reader_variant.NodeType == XmlNodeType.Element)
                                        {
                                            string value = reader_variant.ReadElementContentAsString();
                                            if (attributeId == "f54ProductColor")
                                                option.Color = value;
                                            else if (attributeId == "f54ProductSize")
                                                option.Size = value;
                                        }
                                    }
                                }
                                product.Add(option);
                            }
                        }
                        processedVariants.Add(variantProductId);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name == "variants")
                    {
                        return product;
                    }
                }
            }
            return product;
        }

        // Print the Products
        private static void PrintProductDetails(AdmonisProduct product)
        {
            Console.WriteLine($"Product Name: {product.Name}");
            Console.WriteLine($"Product Description: {product.Description}");
            Console.WriteLine($"Brand: {product.Brand}");
            if (product.Options.Count <= 0) { Console.WriteLine(new string('-', 30)); }

            int optionIndex = 1;
            foreach (var optionList in product.Options)
            {
                Console.WriteLine($"Option Set {optionIndex++}:");

                // Print option details for each option set
                foreach (var option in optionList)
                {
                    Console.WriteLine($"Id: {option.optionName}");
                    Console.WriteLine($"Size: {option.Size}");
                    Console.WriteLine($"Color: {option.Color}");
                }

                Console.WriteLine(new string('-', 30));
            }
        }
    }
}
