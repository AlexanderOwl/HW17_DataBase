Feature: DataBase
	In order to speed up testing
	As a QA automation
	I want the database to be checked automatically

	@create @person
Scenario: Create new person
	Given person data TestName, TestLastName, 100, TestCity	
	When send query to data base
	Then new user created with person data

	@update @person
Scenario: Update person data
	Given update person data TestNameUpdate, TestLastNameUpdate, 12, TestCityUpdate
	When send query to data base
	Then new user created with person data

	@create @order
Scenario: Create order to last person
	Given order sum 9123
	When send query to data base
	Then new order created

	@delete @order
Scenario: Delete order to last person
	Given query to delete orders
	When send query to data base
	Then order is deleted

	@person
Scenario: Person id is unique
	Given select all person id
	Then all person id is unique

	@delete @person
Scenario: Delete last person
	Given query to delete person
	When send query to data base
	Then person is deleted