SELECT * FROM employee AS emp 
WHERE salary > (SELECT salary FROM employee WHERE id=emp.chef_id);

SELECT * FROM employee AS emp 
WHERE salary = (SELECT MAX(salary) FROM employee WHERE department_id=emp.department_id);

SELECT id FROM department AS dep
WHERE (SELECT COUNT(id) FROM employee WHERE department_id = dep.id) <= 3;

SELECT * FROM employee AS emp
WHERE (SELECT department_id FROM employee WHERE id=emp.chef_id) <> emp.department_id;

WITH dep_totals AS (SELECT department_id, SUM(salary) AS salary FROM employee GROUP BY department_id)
SELECT department_id FROM dep_totals
WHERE dep_totals.salary = (SELECT MAX(salary) FROM dep_totals);