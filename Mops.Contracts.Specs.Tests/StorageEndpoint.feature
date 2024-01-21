Feature: StorageEndpoint

A short summary of the feature

@GetDocument
Scenario: Get documents from storage
	Given the '<Connection>' to a storage
	When a list of <Amount> documents is requested
	Then the result should be <Amount> documents

Examples:
	| Connection     | Amount |
	| mock://files=2 | 2      |

@GetDocument
Scenario: A document should have mandatory properties
	Given the '<Connection>' to a storage
	And a document with '4D073D8D-8496-4B32-98E4-ACEA0207BF7E' exists
	When the document with '4D073D8D-8496-4B32-98E4-ACEA0207BF7E' is requested
	Then the document should have the property '<Property>'

Examples:
	| Property     | Connection     |
	| Uuid         | mock://files=2 |
	| Title        | mock://files=2 |
	| Extension    | mock://files=2 |
	| Filemap      | mock://files=2 |
	| Tags         | mock://files=2 |
	| Hash         | mock://files=2 |
	| CreationDate | mock://files=2 |

@GetDocument
Scenario: Get paginated documents from storage
	Given the '<Connection>' to a storage
	And an offset of <Offset> is defined
	When a list of <Amount> documents is requested
	Then the result should be <Amount> documents

Examples:
	| Connection     | Amount | Offset |
	| mock://files=2 | 2      | 10     |

@GetDocument
Scenario: Get a document with an id
	Given the '<Connection>' to a storage
	And a document with '<Id>' exists
	When the document with '<Id>' is requested
	Then the document is returned

Examples:
	| Connection     | Id                                   |
	| mock://files=2 | 4D073D8D-8496-4B32-98E4-ACEA0207BF7E |

@AddDocument
Scenario: A document can be added to the store
	Given the '<Connection>' to a storage
	And a document with '<Id>' is provided
	And a file is provided
	When the document is added
	And the document with '<Id>' is requested
	Then the document is returned

Examples:
	| Connection     | Id                                   |
	| mock://files=2 | 4D073D8D-8496-4B32-98E4-ACEA0207BF7E |

