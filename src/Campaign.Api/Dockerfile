FROM mcr.microsoft.com/dotnet/aspnet:6.0
EXPOSE 80 5000

ARG TIMEZONE
ENV TZ ${TIMEZONE}
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

#RUN apt-get update
#RUN apt-get install -y apt-utils
#RUN apt-get install -y libgdiplus
#RUN apt-get install -y libc6-dev 
#RUN ln -s /usr/lib/libgdiplus.so/usr/lib/gdiplus.dll

COPY . .

ENTRYPOINT ["dotnet", "Campaign.Api.dll"]