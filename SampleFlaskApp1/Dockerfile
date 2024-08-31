FROM python:3.10-alpine
WORKDIR /app

COPY . ./
RUN pip install --no-cache-dir --root-user-action=ignore --upgrade pip
RUN pip install --no-cache-dir --root-user-action=ignore -r requirements.txt 

EXPOSE 5050
CMD ["python3", "main.py"]