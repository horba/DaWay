### **Модель:**
#### **Сущности:**
1.	Пользователь **user** (полная информация)
 *	login
 * password
 *	name
 *	surname
 *	Birth
2.	Связка **bound**
 *	Telegram acc
 *	Instagram acc
 *	Instagram password
3.	Пользователь **authorization** (только авторизация)
 *	login
 *	password
4.	Фильтр **filter**
 *	filter

*жирным - имя класса


### **Запросы:**

**1.	Чтение:**
 *	запрос логина и пароля. (сущ. 3)
 *	запрос всех связок пользователя.  (сущ. 2[])
 *	Список фильтров связки. (сущ. 4[])
 *	запрос полной информации. (сущ. 1)

**2. Запись/изменение/удаление:**
 *	Добавить пользователя. (сущ. 1)
 *	добавить связку. (сущ. 2)
 *	добавить фильтр. (сущ. 4)

*сущ - номер соответствующей сущности.

**[] - список сущностей

### **Слой общения с базой:**
**class UserRepository** содержит функции, которые выполняют запросы.
