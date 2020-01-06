namespace SharpApi.RestApi.Databases

open SharpApi.Core.Models

type IDatabase = 
  abstract member Insert: DocumentProperty -> DocumentObject -> DocumentObject option
  abstract member Remove: DocumentProperty -> DocumentObject option
  abstract member Get: DocumentProperty -> DocumentObject option
  abstract member Get: unit -> DocumentObject list
  abstract member Exists: DocumentProperty -> bool
  abstract member Update: DocumentProperty -> DocumentObject -> DocumentObject option