FROM python:3.10-slim
WORKDIR /opt/app
COPY . ./
RUN ls
RUN pip install -r requirements.txt
CMD ["python", "./main.py"]