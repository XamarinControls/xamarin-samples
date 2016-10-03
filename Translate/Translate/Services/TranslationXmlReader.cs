using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Translate.Core.Contracts;
using Translate.Core.Model;

namespace Translate.Services
{
    public class TranslationXmlReader : ITranslationXmlReader
    {
        public async Task<IEnumerable<TranslationRecord>> LoadTranslationsAsync()
        {
            return await Task.Run(() =>
            {
                var xmlString = System.IO.File.ReadAllText("translations.xml");
                XDocument doc = XDocument.Parse(xmlString);

                var results = new List<TranslationRecord>();
                foreach (XElement element in doc.Descendants("RECORD"))
                {

                    var translationRecord = GetRecord(element);

                    foreach (var linkElement in element.Descendants("LINK"))
                    {
                        var linkRecord = GetRecord(linkElement);
                        linkRecord.ParentRecord = translationRecord;
                        translationRecord.TranslationLinks.Add(linkRecord);
                    }

                    results.Add(translationRecord);
                }

                return results;
            });
        }

        private static TranslationRecord GetRecord(XElement element)
        {
            var wordAttribute = element.Attribute("word");
            var cultureAttribute = element.Attribute("culture");
            if (wordAttribute != null && cultureAttribute != null)
            {
                return new TranslationRecord()
                {
                    Entry = new TranslationEntry(wordAttribute.Value, cultureAttribute.Value)
                };
            }

            return null;
        }
    }
}