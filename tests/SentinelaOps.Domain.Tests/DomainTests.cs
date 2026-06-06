namespace SentinelaOps.Domain.Tests;

using SentinelaOps.Domain.Core;
using Xunit;

public class EventIdTests
{
    [Fact]
    public void Create_WithValidInput_ReturnsEventId()
    {
        // Arrange
        var cameraId = "cam-001";
        var timestamp = new DateTime(2024, 1, 15, 10, 30, 45);
        var sequence = 1;

        // Act
        var eventId = EventId.Create(cameraId, timestamp, sequence);

        // Assert
        Assert.NotNull(eventId);
        Assert.Equal("cam-001_20240115103045_0001", eventId.ToString());
    }

    [Fact]
    public void Create_WithEmptyCameraId_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => EventId.Create("", DateTime.UtcNow, 1));
    }

    [Fact]
    public void Parse_WithValidString_ReturnsEventId()
    {
        // Arrange
        var eventIdString = "cam-001_20240115103045_0001";

        // Act
        var eventId = EventId.Parse(eventIdString);

        // Assert
        Assert.NotNull(eventId);
        Assert.Equal(eventIdString, eventId.ToString());
    }

    [Fact]
    public void Equals_WithSameValue_ReturnsTrue()
    {
        // Arrange
        var eventId1 = EventId.Create("cam-001", DateTime.UtcNow, 1);
        var eventId2 = EventId.Parse(eventId1.ToString());

        // Act & Assert
        Assert.Equal(eventId1, eventId2);
    }

    [Fact]
    public void Equals_WithDifferentValue_ReturnsFalse()
    {
        // Arrange
        var eventId1 = EventId.Create("cam-001", DateTime.UtcNow, 1);
        var eventId2 = EventId.Create("cam-002", DateTime.UtcNow, 1);

        // Act & Assert
        Assert.NotEqual(eventId1, eventId2);
    }
}

public class CorrelationIdTests
{
    [Fact]
    public void Create_ReturnsNewGuidEachTime()
    {
        // Act
        var correlationId1 = CorrelationId.Create();
        var correlationId2 = CorrelationId.Create();

        // Assert
        Assert.NotEqual(correlationId1.Value, correlationId2.Value);
    }

    [Fact]
    public void Parse_WithValidGuid_ReturnsCorrelationId()
    {
        // Arrange
        var guid = Guid.NewGuid().ToString();

        // Act
        var correlationId = CorrelationId.Parse(guid);

        // Assert
        Assert.NotNull(correlationId);
        Assert.Equal(guid, correlationId.ToString());
    }

    [Fact]
    public void Parse_WithInvalidGuid_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CorrelationId.Parse("not-a-guid"));
    }
}

public class ConfidenceTests
{
    [Fact]
    public void Create_WithValidValue_ReturnsConfidence()
    {
        // Act
        var confidence = Confidence.Create(0.85);

        // Assert
        Assert.Equal(0.85, confidence.Value);
        Assert.Equal(85, confidence.Percentage);
    }

    [Fact]
    public void Create_WithValueBelowZero_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Confidence.Create(-0.1));
    }

    [Fact]
    public void Create_WithValueAboveOne_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => Confidence.Create(1.1));
    }

    [Fact]
    public void FromPercentage_WithValidPercentage_ReturnsConfidence()
    {
        // Act
        var confidence = Confidence.FromPercentage(85);

        // Assert
        Assert.Equal(0.85, confidence.Value, precision: 4);
        Assert.Equal(85, confidence.Percentage, precision: 2);
    }

    [Fact]
    public void Comparison_OperatorsWork()
    {
        // Arrange
        var confidence1 = Confidence.Create(0.5);
        var confidence2 = Confidence.Create(0.7);

        // Act & Assert
        Assert.True(confidence1 < confidence2);
        Assert.True(confidence2 > confidence1);
        Assert.False(confidence1 == confidence2);
    }
}

public class ClassificationTests
{
    [Fact]
    public void Valid_StaticProperty_ReturnsValidClassification()
    {
        // Act & Assert
        Assert.Equal(ClassificationValue.Valid, Classification.Valid.Value);
    }

    [Fact]
    public void IsThreat_ForSuspicious_ReturnsTrue()
    {
        // Act & Assert
        Assert.True(Classification.Suspicious.IsThreat);
    }

    [Fact]
    public void RequiresHumanReview_ForHumanReviewRequired_ReturnsTrue()
    {
        // Act & Assert
        Assert.True(Classification.HumanReviewRequired.RequiresHumanReview);
    }

    [Fact]
    public void Parse_WithValidString_ReturnsClassification()
    {
        // Act
        var classification = Classification.Parse("Suspicious");

        // Assert
        Assert.Equal(Classification.Suspicious, classification);
    }
}

public class MonitoringEventTests
{
    [Fact]
    public void Create_WithValidInput_ReturnsMonitoringEvent()
    {
        // Arrange
        var eventId = EventId.Create("cam-001", DateTime.UtcNow, 1);
        var correlationId = CorrelationId.Create();
        var metadata = new EventMetadata("zone-1", "sensor-001", EventSensitivity.High, DateTime.UtcNow);

        // Act
        var monitoringEvent = MonitoringEvent.Create(eventId, correlationId, metadata, DateTime.UtcNow);

        // Assert
        Assert.NotNull(monitoringEvent);
        Assert.Equal(EventStatus.Received, monitoringEvent.Status);
        Assert.Equal(eventId, monitoringEvent.EventId);
        Assert.Equal(correlationId, monitoringEvent.CorrelationId);
    }

    [Fact]
    public void Create_RaisesDomainEvent()
    {
        // Arrange
        var eventId = EventId.Create("cam-001", DateTime.UtcNow, 1);
        var correlationId = CorrelationId.Create();
        var metadata = new EventMetadata("zone-1", "sensor-001", EventSensitivity.High, DateTime.UtcNow);

        // Act
        var monitoringEvent = MonitoringEvent.Create(eventId, correlationId, metadata, DateTime.UtcNow);

        // Assert
        var domainEvents = monitoringEvent.GetDomainEvents();
        Assert.Single(domainEvents);
        Assert.IsType<EventReceivedDomainEvent>(domainEvents.First());
    }

    [Fact]
    public void StartAnalysis_ChangesStatusToProcessing()
    {
        // Arrange
        var eventId = EventId.Create("cam-001", DateTime.UtcNow, 1);
        var correlationId = CorrelationId.Create();
        var metadata = new EventMetadata("zone-1", "sensor-001", EventSensitivity.High, DateTime.UtcNow);
        var monitoringEvent = MonitoringEvent.Create(eventId, correlationId, metadata, DateTime.UtcNow);

        // Act
        monitoringEvent.StartAnalysis();

        // Assert
        Assert.Equal(EventStatus.Processing, monitoringEvent.Status);
    }

    [Fact]
    public void StartAnalysis_FromProcessing_ThrowsException()
    {
        // Arrange
        var eventId = EventId.Create("cam-001", DateTime.UtcNow, 1);
        var correlationId = CorrelationId.Create();
        var metadata = new EventMetadata("zone-1", "sensor-001", EventSensitivity.High, DateTime.UtcNow);
        var monitoringEvent = MonitoringEvent.Create(eventId, correlationId, metadata, DateTime.UtcNow);
        monitoringEvent.StartAnalysis();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => monitoringEvent.StartAnalysis());
    }

    [Fact]
    public void CompleteAnalysis_WithValidResult_UpdatesEvent()
    {
        // Arrange
        var eventId = EventId.Create("cam-001", DateTime.UtcNow, 1);
        var correlationId = CorrelationId.Create();
        var metadata = new EventMetadata("zone-1", "sensor-001", EventSensitivity.High, DateTime.UtcNow);
        var monitoringEvent = MonitoringEvent.Create(eventId, correlationId, metadata, DateTime.UtcNow);
        monitoringEvent.StartAnalysis();

        var classification = Classification.Suspicious;
        var confidence = Confidence.Create(0.87);
        var justification = Justification.Create("Suspicious behavior detected");
        var evidence = new List<string> { "Evidence 1", "Evidence 2" };

        // Act
        monitoringEvent.CompleteAnalysis(classification, confidence, justification, evidence);

        // Assert
        Assert.Equal(EventStatus.Analyzed, monitoringEvent.Status);
        var result = monitoringEvent.GetAnalysisResult();
        Assert.NotNull(result);
        Assert.Equal(classification, result.Classification);
        Assert.Equal(confidence, result.Confidence);
    }

    [Fact]
    public void CompleteAnalysis_FromReceived_ThrowsException()
    {
        // Arrange
        var eventId = EventId.Create("cam-001", DateTime.UtcNow, 1);
        var correlationId = CorrelationId.Create();
        var metadata = new EventMetadata("zone-1", "sensor-001", EventSensitivity.High, DateTime.UtcNow);
        var monitoringEvent = MonitoringEvent.Create(eventId, correlationId, metadata, DateTime.UtcNow);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => monitoringEvent.CompleteAnalysis(
                Classification.Suspicious,
                Confidence.Create(0.87),
                Justification.Create("Test"),
                new List<string>()
            )
        );
    }

    [Fact]
    public void GetAnalysisResult_BeforeAnalysis_ReturnsNull()
    {
        // Arrange
        var eventId = EventId.Create("cam-001", DateTime.UtcNow, 1);
        var correlationId = CorrelationId.Create();
        var metadata = new EventMetadata("zone-1", "sensor-001", EventSensitivity.High, DateTime.UtcNow);
        var monitoringEvent = MonitoringEvent.Create(eventId, correlationId, metadata, DateTime.UtcNow);

        // Act
        var result = monitoringEvent.GetAnalysisResult();

        // Assert
        Assert.Null(result);
    }
}
