import json
from pathlib import Path
p=Path('database/quizlab.sql')
s=p.read_text(encoding='utf-8-sig').split('START TRANSACTION;')[0]+'START TRANSACTION;\n'
def text(v): return "CONVERT(0x"+v.encode('utf-8').hex()+" USING utf8mb4)"
data=json.loads(Path('QuizLab/Data/quizzes.json').read_text(encoding='utf-8-sig'))
for q in data:
 id=text(q['Id']); title=text(q['Title'])
 s+=f'INSERT INTO quizzes (id,title) VALUES ({id},{title}) ON DUPLICATE KEY UPDATE title=VALUES(title);\n'
 for i,item in enumerate(q['Questions']):
  body=text(item['Text']); correct=item['CorrectIndex']; assert 0<=correct<len(item['Options'])
  s+=f'INSERT INTO questions (quiz_id,question_index,question_text,correct_index) VALUES ({id},{i},{body},{correct}) ON DUPLICATE KEY UPDATE id=LAST_INSERT_ID(id),question_text=VALUES(question_text),correct_index=VALUES(correct_index);\nSET @question_id=LAST_INSERT_ID();\n'
  for j,option in enumerate(item['Options']):
   s+=f'INSERT INTO answer_options (question_id,option_index,option_text) VALUES (@question_id,{j},{text(option)}) ON DUPLICATE KEY UPDATE option_text=VALUES(option_text);\n'
s+='COMMIT;\n';p.write_text(s,encoding='utf-8')
