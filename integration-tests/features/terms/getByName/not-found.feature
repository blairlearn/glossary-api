Feature: GetByName returns a status 404 when the pretty URL name is not found.

    Background:
        * url apiHost

    Scenario Outline: Without fallback logic, valid dictionary, invalid pretty url
        Test for dictionary: '<dictionary>', audience: '<audience>', language: '<language>'

        Given path 'Terms', dictionary, audience, language, 'abcdefghijk'
        And param useFallback = false
        When method get
        Then status 404
        And match response.Message == expected

        Examples:
            | dictionary | audience           | language | expected                                                                                                         |
            | cancer.gov | Patient            | en       | No match for dictionary 'cancer.gov', audience 'Patient', language 'en' and pretty url 'abcdefghijk'.            |
            | genetics   | Patient            | en       | No match for dictionary 'genetics', audience 'Patient', language 'en' and pretty url 'abcdefghijk'.              |
            | notset     | Patient            | en       | No match for dictionary 'notset', audience 'Patient', language 'en' and pretty url 'abcdefghijk'.                |
            | cancer.gov | HealthProfessional | en       | No match for dictionary 'cancer.gov', audience 'HealthProfessional', language 'en' and pretty url 'abcdefghijk'. |
            | genetics   | HealthProfessional | en       | No match for dictionary 'genetics', audience 'HealthProfessional', language 'en' and pretty url 'abcdefghijk'.   |
            | notset     | HealthProfessional | en       | No match for dictionary 'notset', audience 'HealthProfessional', language 'en' and pretty url 'abcdefghijk'.     |
            | cancer.gov | Patient            | es       | No match for dictionary 'cancer.gov', audience 'Patient', language 'es' and pretty url 'abcdefghijk'.            |
            | genetics   | Patient            | es       | No match for dictionary 'genetics', audience 'Patient', language 'es' and pretty url 'abcdefghijk'.              |
            | notset     | Patient            | es       | No match for dictionary 'notset', audience 'Patient', language 'es' and pretty url 'abcdefghijk'.                |
            | cancer.gov | HealthProfessional | es       | No match for dictionary 'cancer.gov', audience 'HealthProfessional', language 'es' and pretty url 'abcdefghijk'. |
            | genetics   | HealthProfessional | es       | No match for dictionary 'genetics', audience 'HealthProfessional', language 'es' and pretty url 'abcdefghijk'.   |
            | notset     | HealthProfessional | es       | No match for dictionary 'notset', audience 'HealthProfessional', language 'es' and pretty url 'abcdefghijk'.     |


    Scenario Outline: Without fallback logic, unknown dictionary, valid pretty url
        Test for audience: '<audience>', language: '<language>'

        Given path 'Terms', 'unknown', audience, language, 'lung'
        And param useFallback = false
        When method get
        Then status 404
        And match response.Message == expected

        Examples:
            | audience           | language | id    | expected                                                                                        |
            | Patient            | en       | 43966 | No match for dictionary 'unknown', audience 'Patient', language 'en' and pretty url 'lung'.            |
            | HealthProfessional | en       | 43966 | No match for dictionary 'unknown', audience 'HealthProfessional', language 'en' and pretty url 'lung'. |
            | Patient            | es       | 43966 | No match for dictionary 'unknown', audience 'Patient', language 'es' and pretty url 'lung'.            |
            | HealthProfessional | es       | 43966 | No match for dictionary 'unknown', audience 'HealthProfessional', language 'es' and pretty url 'lung'. |


    Scenario Outline: Using fallback logic, and valid dictionary, invalid pretty url.
        Test for dictionary: '<dictionary>', audience: '<audience>', language: '<language>'

        Given path 'Terms', dictionary, audience, language, 'abcdefghijk'
        And param useFallback = true
        When method get
        Then status 404
        And match response.Message == expected

        Examples:
            | dictionary | audience           | language | expected                                                                                                        |
            | cancer.gov | Patient            | en       | Could not find fallback term with pretty URL name 'abcdefghijk' for any combination of dictionary and audience. |
            | genetics   | Patient            | en       | Could not find fallback term with pretty URL name 'abcdefghijk' for any combination of dictionary and audience. |
            | notset     | Patient            | en       | Could not find fallback term with pretty URL name 'abcdefghijk' for any combination of dictionary and audience. |
            | cancer.gov | HealthProfessional | en       | Could not find fallback term with pretty URL name 'abcdefghijk' for any combination of dictionary and audience. |
            | genetics   | HealthProfessional | en       | Could not find fallback term with pretty URL name 'abcdefghijk' for any combination of dictionary and audience. |
            | notset     | HealthProfessional | en       | Could not find fallback term with pretty URL name 'abcdefghijk' for any combination of dictionary and audience. |
            | cancer.gov | Patient            | es       | Could not find fallback term with pretty URL name 'abcdefghijk' for any combination of dictionary and audience. |
            | genetics   | Patient            | es       | Could not find fallback term with pretty URL name 'abcdefghijk' for any combination of dictionary and audience. |
            | notset     | Patient            | es       | Could not find fallback term with pretty URL name 'abcdefghijk' for any combination of dictionary and audience. |
            | cancer.gov | HealthProfessional | es       | Could not find fallback term with pretty URL name 'abcdefghijk' for any combination of dictionary and audience. |
            | genetics   | HealthProfessional | es       | Could not find fallback term with pretty URL name 'abcdefghijk' for any combination of dictionary and audience. |
            | notset     | HealthProfessional | es       | Could not find fallback term with pretty URL name 'abcdefghijk' for any combination of dictionary and audience. |


    Scenario Outline: Using fallback logic, unknown dictionary, valid pretty url
        Test for audience: '<audience>', language: '<language>'

        Given path 'Terms', 'unknown', audience, language, 'lung'
        And param useFallback = true
        When method get
        Then status 404
        And match response.Message == expected

        Examples:
            | audience           | language | id    | expected                                                                                                 |
            | Patient            | en       | 43966 | Could not find initial fallback combination with dictionary 'unknown' and audience 'Patient'.            |
            | HealthProfessional | en       | 43966 | Could not find initial fallback combination with dictionary 'unknown' and audience 'HealthProfessional'. |
            | Patient            | es       | 43966 | Could not find initial fallback combination with dictionary 'unknown' and audience 'Patient'.            |
            | HealthProfessional | es       | 43966 | Could not find initial fallback combination with dictionary 'unknown' and audience 'HealthProfessional'. |
